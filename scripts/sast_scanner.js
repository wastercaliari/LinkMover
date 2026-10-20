/**
 * SAST & Secret Detection Engine — Shift-Left DevSecOps
 * Scanner estático nativo de vulnerabilidades OWASP Top 10, CWEs e segredos em texto plano.
 * Em estrita conformidade com a Regra 5, Regra 12 e NIST SSDF SP 800-218.
 */

const fs = require('node:fs');
const path = require('node:path');

const SECURITY_RULES = [
  {
    id: 'DEVSEC-CWE-78',
    cwe: 'CWE-78',
    name: 'Command Injection',
    severity: 'CRITICAL',
    pattern: /(?:child_process\s*\.\s*exec\s*\(|execSync\s*\(|system\s*\(|popen\s*\(|`[^`]*\$\{.*?\}[^`]*`\s*(?:\|\||&&)?\s*exec)/i,
    message: 'Execução de comando via shell detectada. Utilize execFile ou spawn com argumentos em array sanitizados.',
    remediation: 'Substitua child_process.exec por child_process.execFile ou spawn sem invocação de shell intermediário.'
  },
  {
    id: 'DEVSEC-CWE-89',
    cwe: 'CWE-89',
    name: 'SQL Injection',
    severity: 'CRITICAL',
    pattern: /(?:SELECT|INSERT|UPDATE|DELETE|DROP)\s+[\s\S]*?['"`]\s*\+\s*[a-zA-Z0-9_$]|\$\{.*?\}.*(?:FROM|WHERE|VALUES)|(?:SELECT|INSERT|UPDATE|DELETE|DROP)[^;]*?\+\s*[a-zA-Z0-9_$]/i,
    message: 'Interpolação dinâmica de parâmetros em instrução SQL detectada.',
    remediation: 'Utilize consultas parametrizadas (Prepared Statements) ou query builders com bind de parâmetros.'
  },
  {
    id: 'DEVSEC-CWE-1333',
    cwe: 'CWE-1333',
    name: 'ReDoS (Regular Expression Denial of Service)',
    severity: 'HIGH',
    pattern: /\/([^\/\\]*(\([^\(\)]*[\+\*][^\(\)]*\)[\+\*])[^\/\\]*)\/[gimsuy]*/,
    message: 'Expressão regular com quantificadores aninhados propensa a catastrophic backtracking exponencial.',
    remediation: 'Refatore a regex para eliminar quantificadores aninhados ou substitua por autômato linear / busca determinística.'
  },
  {
    id: 'DEVSEC-CWE-79',
    cwe: 'CWE-79',
    name: 'Cross-Site Scripting (XSS)',
    severity: 'HIGH',
    pattern: /(?:innerHTML\s*=|dangerouslySetInnerHTML\s*=|document\s*\.\s*write\s*\()/i,
    message: 'Atribuição insegura de HTML direto no DOM sem sanitização contextual.',
    remediation: 'Utilize textContent ou bibliotecas canônicas de sanitização estrita (DOMPurify).'
  },
  {
    id: 'DEVSEC-CWE-798-SECRET',
    cwe: 'CWE-798',
    name: 'Hardcoded Credentials & API Keys',
    severity: 'CRITICAL',
    pattern: /(?:(?:AKIA[0-9A-Z]{16})|(?:ghp_[A-Za-z0-9]{36})|(?:xox[baprs]-[0-9a-zA-Z]{10,48})|(?:-----BEGIN\s+PRIVATE\s+KEY-----)|(?:(?:api[_-]?key|secret|password|auth[_-]?token)\s*[:=]\s*['"][A-Za-z0-9_\-\.+=]{16,}['"]))/i,
    message: 'Credencial, token de acesso ou chave privada em texto plano embutida no código-fonte.',
    remediation: 'Remova imediatamente o segredo, revogue a credencial e carregue via variáveis de ambiente seguras (process.env).'
  },
  {
    id: 'DEVSEC-CWE-798-EMAIL',
    cwe: 'CWE-798',
    name: 'Hardcoded Email or Fallback Credential',
    severity: 'CRITICAL',
    pattern: /(?:(?:email|user|account)\s*(?:[:=]|\|\|)\s*['"][a-zA-Z0-9._%+-]+@(gmail|hotmail|outlook|yahoo|[a-zA-Z0-9.-]+)\.[a-zA-Z]{2,}['"])/i,
    message: 'Endereço de email hardcoded ou utilizado como fallback no código-fonte.',
    remediation: 'Remova o email estático e obtenha o valor exclusivamente de process.env (ex: process.env.ATLASSIAN_EMAIL).'
  },
  {
    id: 'DEVSEC-CWE-798-TENANT-DOMAIN',
    cwe: 'CWE-798',
    name: 'Hardcoded Atlassian Tenant Domain',
    severity: 'CRITICAL',
    pattern: /https?:\/\/[a-zA-Z0-9-]+\.atlassian\.net/i,
    message: 'Domínio privado de tenant Atlassian hardcoded no código.',
    remediation: 'Substitua por variável de ambiente dinâmica (ex: process.env.ATLASSIAN_DOMAIN).'
  },
  {
    id: 'DEVSEC-CWE-798-EXTERNAL-FILE',
    cwe: 'CWE-798',
    name: 'External Account File Reference',
    severity: 'CRITICAL',
    pattern: /['"][^'"]*(?:Atlassian Account|Railway Account|Token\.txt|Account\.txt)['"]/i,
    message: 'Leitura de arquivo de credenciais externo ou fora do escopo do repositório.',
    remediation: 'Elimine acessos a arquivos locais de conta. Utilize variáveis de ambiente ou .env protegido por .gitignore.'
  },
  {
    id: 'DEVSEC-CWE-95',
    cwe: 'CWE-95',
    name: 'Dynamic Code Evaluation',
    severity: 'CRITICAL',
    pattern: /\beval\s*\(|\bnew\s+Function\s*\(/,
    message: 'Uso de eval() ou new Function() que permite execução arbitrária de código.',
    remediation: 'Substitua por parsers seguros de JSON ou mapeamentos estáticos de funções.'
  }
];

class SastScanner {
  constructor(rules = SECURITY_RULES) {
    this.rules = rules;
  }

  scanCode(code, filename = 'snippet.js') {
    const findings = [];
    const lines = code.split('\n');

    for (let lineIdx = 0; lineIdx < lines.length; lineIdx++) {
      const lineText = lines[lineIdx];

      for (const rule of this.rules) {
        if (rule.pattern.test(lineText)) {
          findings.push({
            ruleId: rule.id,
            name: rule.name,
            cwe: rule.cwe,
            severity: rule.severity,
            filename,
            line: lineIdx + 1,
            snippet: lineText.trim().substring(0, 100),
            message: rule.message,
            remediation: rule.remediation
          });
        }
      }
    }

    const hasCritical = findings.some(f => f.severity === 'CRITICAL');
    const hasHigh = findings.some(f => f.severity === 'HIGH');
    const status = (hasCritical || hasHigh) ? 'FAILED' : (findings.length > 0 ? 'WARNING' : 'PASSED');

    return {
      status,
      filename,
      totalFindings: findings.length,
      criticalCount: findings.filter(f => f.severity === 'CRITICAL').length,
      highCount: findings.filter(f => f.severity === 'HIGH').length,
      mediumCount: findings.filter(f => f.severity === 'MEDIUM').length,
      findings
    };
  }

  scanDirectory(dirPath, extensions = ['.js', '.ts', '.jsx', '.tsx', '.json', '.md']) {
    let allFindings = [];
    let filesScanned = 0;

    const walk = (dir) => {
      const entries = fs.readdirSync(dir, { withFileTypes: true });
      for (const entry of entries) {
        const fullPath = path.join(dir, entry.name);
        if (entry.isDirectory()) {
          if (!['node_modules', '.git', 'dist', 'build', '.gemini'].includes(entry.name)) {
            walk(fullPath);
          }
        } else if (entry.isFile()) {
          const ext = path.extname(entry.name);
          if (extensions.includes(ext)) {
            filesScanned++;
            const content = fs.readFileSync(fullPath, 'utf8');
            const result = this.scanCode(content, path.relative(dirPath, fullPath));
            if (result.findings.length > 0) {
              allFindings.push(...result.findings);
            }
          }
        }
      }
    };

    walk(dirPath);

    return {
      filesScanned,
      totalFindings: allFindings.length,
      criticalCount: allFindings.filter(f => f.severity === 'CRITICAL').length,
      highCount: allFindings.filter(f => f.severity === 'HIGH').length,
      findings: allFindings,
      status: allFindings.some(f => f.severity === 'CRITICAL' || f.severity === 'HIGH') ? 'FAILED' : 'PASSED'
    };
  }
}

module.exports = {
  SastScanner,
  SECURITY_RULES
};
