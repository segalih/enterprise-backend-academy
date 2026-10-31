import React, { useState, useEffect, useRef, useContext, createContext } from 'react';
import ReactMarkdown from 'react-markdown';
import remarkGfm from 'remark-gfm';
import { 
  ArrowLeft, 
  ArrowRight, 
  Clock, 
  Layers, 
  CheckCircle2, 
  HelpCircle, 
  Award, 
  Sparkles,
  BookOpen,
  Copy,
  Check,
  Terminal as TerminalIcon
} from 'lucide-react';
import { LessonDetail } from '../types/academy';
import { MonacoCodeEditor } from '../components/editor/MonacoCodeEditor';
import { QuizView } from '../components/quiz/QuizView';
import { ChallengeView } from '../components/challenge/ChallengeView';
import { BinarySimulator } from '../components/simulators/BinarySimulator';
import { StackHeapSimulator } from '../components/simulators/StackHeapSimulator';
import { GCSimulator } from '../components/simulators/GCSimulator';
import { AsyncStateMachineSimulator } from '../components/simulators/AsyncStateMachineSimulator';
import { BTreeSimulator } from '../components/simulators/BTreeSimulator';
import { DeadlockSimulator } from '../components/simulators/DeadlockSimulator';
import { MermaidDiagram } from '../components/common/MermaidDiagram';

const PreContext = createContext<boolean>(false);

const CSHARP_KEYWORDS = new Set([
  'abstract', 'as', 'async', 'await', 'base', 'break', 'case', 'catch', 'checked',
  'class', 'const', 'continue', 'default', 'delegate', 'do', 'else', 'enum', 'event',
  'explicit', 'extern', 'false', 'finally', 'fixed', 'for', 'foreach', 'goto', 'if',
  'implicit', 'in', 'interface', 'internal', 'is', 'lock', 'namespace', 'new', 'null',
  'operator', 'out', 'override', 'params', 'private', 'protected', 'public', 'readonly',
  'record', 'ref', 'return', 'sealed', 'sizeof', 'stackalloc', 'static', 'struct',
  'switch', 'this', 'throw', 'true', 'try', 'typeof', 'unchecked', 'unsafe', 'using',
  'var', 'virtual', 'void', 'volatile', 'when', 'where', 'while', 'with', 'yield',
  'get', 'set', 'init', 'required', 'scoped'
]);

const CSHARP_TYPES = new Set([
  'bool', 'byte', 'char', 'decimal', 'double', 'float', 'int', 'long', 'nint', 'nuint',
  'object', 'sbyte', 'short', 'string', 'uint', 'ulong', 'ushort', 'Span', 'ReadOnlySpan',
  'Memory', 'ReadOnlyMemory', 'Task', 'ValueTask', 'IEnumerable', 'IQueryable', 'List',
  'Dictionary', 'HashSet', 'Queue', 'Stack', 'Console', 'Environment', 'GC', 'Stopwatch',
  'Guid', 'DateTime', 'TimeSpan', 'Exception', 'InvalidOperationException', 'ArgumentException',
  'HttpClient', 'HttpRequestMessage', 'HttpResponseMessage', 'CancellationToken', 'Channel',
  'Interlocked', 'SemaphoreSlim', 'DbContext', 'StringBuilder'
]);

function highlightCSharpLine(line: string, lineIdx: number): React.ReactNode {
  // Check for full-line or trailing comment outside quotes
  let inString = false;
  let commentStart = -1;
  for (let i = 0; i < line.length; i++) {
    if (line[i] === '"' && (i === 0 || line[i - 1] !== '\\')) {
      inString = !inString;
    } else if (!inString && line[i] === '/' && line[i + 1] === '/') {
      commentStart = i;
      break;
    }
  }

  const codePart = commentStart >= 0 ? line.slice(0, commentStart) : line;
  const commentPart = commentStart >= 0 ? line.slice(commentStart) : '';

  const tokenRegex = /(\$?"(?:[^"\\]|\\.)*"|'(?:[^'\\]|\\.)*'|\b\d+(?:_\d+)*(?:\.\d+)?[mMdDfFlLuU]?\b|\b[A-Za-z_][A-Za-z0-9_]*\b)/g;
  const parts: React.ReactNode[] = [];
  let lastIndex = 0;
  let match: RegExpExecArray | null;
  let tokenIdx = 0;

  while ((match = tokenRegex.exec(codePart)) !== null) {
    if (match.index > lastIndex) {
      parts.push(codePart.slice(lastIndex, match.index));
    }
    const token = match[0];
    if (token.startsWith('"') || token.startsWith('$"') || token.startsWith("'")) {
      parts.push(<span key={`${lineIdx}-t-${tokenIdx++}`} className="text-amber-300">{token}</span>);
    } else if (/^\d/.test(token)) {
      parts.push(<span key={`${lineIdx}-t-${tokenIdx++}`} className="text-purple-300">{token}</span>);
    } else if (CSHARP_KEYWORDS.has(token)) {
      parts.push(<span key={`${lineIdx}-t-${tokenIdx++}`} className="text-sky-400 font-semibold">{token}</span>);
    } else if (CSHARP_TYPES.has(token) || (/^[A-Z][A-Za-z0-9_]*$/.test(token) && token.length > 1)) {
      parts.push(<span key={`${lineIdx}-t-${tokenIdx++}`} className="text-teal-300">{token}</span>);
    } else {
      parts.push(token);
    }
    lastIndex = tokenRegex.lastIndex;
  }

  if (lastIndex < codePart.length) {
    parts.push(codePart.slice(lastIndex));
  }

  if (commentPart) {
    parts.push(<span key={`${lineIdx}-comment`} className="text-slate-500 italic">{commentPart}</span>);
  }

  return parts;
}

function renderHighlightedCode(code: string, lang: string): React.ReactNode {
  const normalized = lang.toLowerCase();
  if (normalized !== 'csharp' && normalized !== 'cs') {
    return code;
  }
  const lines = code.split('\n');
  return lines.map((line, i) => (
    <React.Fragment key={i}>
      {highlightCSharpLine(line, i)}
      {i < lines.length - 1 ? '\n' : ''}
    </React.Fragment>
  ));
}

function isAsciiDiagramContent(code: string): boolean {
  return /[+─│┌┐└┘├┤┬┴┼▲▼◄►]|-->|<--|\+---|\[.*\]\s*-->/.test(code);
}

interface CodeBlockProps {
  className?: string;
  children?: React.ReactNode;
}

export const MarkdownCodeBlock: React.FC<CodeBlockProps> = React.memo(({ className, children }) => {
  const [copied, setCopied] = useState(false);
  const rawCode = String(children ?? '').replace(/\n$/, '');
  const rawLang = className?.replace(/^language-/, '').trim() || '';

  if (rawLang === 'mermaid') {
    return <MermaidDiagram code={rawCode} />;
  }

  const isDiagram = (!rawLang || rawLang === 'text' || rawLang === 'plaintext' || rawLang === 'ascii' || rawLang === 'diagram') && isAsciiDiagramContent(rawCode);
  const displayLang = isDiagram
    ? 'diagram / skema'
    : rawLang || 'text';

  const handleCopy = async () => {
    try {
      await navigator.clipboard.writeText(rawCode);
      setCopied(true);
      setTimeout(() => setCopied(false), 2000);
    } catch {
      // Fallback ignore if clipboard unavailable
    }
  };

  return (
    <div className="my-5 rounded-xl overflow-hidden border border-slate-800/90 bg-[#070b14] shadow-lg shadow-black/25">
      <div className="bg-slate-900/95 px-4 py-2 text-[11px] font-mono text-slate-400 border-b border-slate-800/90 flex items-center justify-between select-none">
        <div className="flex items-center space-x-2">
          <TerminalIcon className="w-3.5 h-3.5 text-blue-400/80 shrink-0" />
          <span className="uppercase tracking-wider font-semibold text-slate-300">
            {displayLang}
          </span>
        </div>
        <button
          type="button"
          onClick={handleCopy}
          title="Salin kode"
          className="flex items-center space-x-1.5 px-2 py-0.5 rounded bg-slate-800/80 hover:bg-slate-700/80 text-slate-300 hover:text-white border border-slate-700/60 transition text-[11px]"
        >
          {copied ? (
            <>
              <Check className="w-3 h-3 text-emerald-400" />
              <span className="text-emerald-300">Tersalin</span>
            </>
          ) : (
            <>
              <Copy className="w-3 h-3 text-slate-400" />
              <span>Salin</span>
            </>
          )}
        </button>
      </div>
      <pre className="p-4 sm:p-5 overflow-x-auto text-[13px] leading-[1.6] font-mono text-slate-100 whitespace-pre m-0 selection:bg-blue-500/30">
        <code className={className ? `${className} block font-mono whitespace-pre` : 'block font-mono whitespace-pre'}>
          {renderHighlightedCode(rawCode, rawLang)}
        </code>
      </pre>
    </div>
  );
});

const MarkdownCode: React.FC<any> = ({ className, children, ...props }) => {
  const isBlock = useContext(PreContext);
  if (!isBlock) {
    return (
      <code
        className="bg-slate-800/90 text-sky-300 font-mono text-[0.8125rem] px-1.5 py-0.5 rounded-md border border-slate-700/70 font-medium"
        {...props}
      >
        {children}
      </code>
    );
  }
  return <MarkdownCodeBlock className={className}>{children}</MarkdownCodeBlock>;
};

function extractNodeText(node: React.ReactNode): string {
  if (node === null || node === undefined || typeof node === 'boolean') return '';
  if (typeof node === 'string' || typeof node === 'number') return String(node);
  if (Array.isArray(node)) return node.map(extractNodeText).join('');
  if (React.isValidElement(node)) {
    return extractNodeText((node.props as { children?: React.ReactNode }).children);
  }
  return '';
}

function toHeadingSlug(children: React.ReactNode): string {
  return extractNodeText(children)
    .trim()
    .toLowerCase()
    .replace(/[^a-z0-9]+/g, '-')
    .replace(/^-+|-+$/g, '');
}

const STATIC_MARKDOWN_COMPONENTS: Record<string, React.FC<any>> = {
  h1: ({ children }) => (
    <h1 className="text-2xl font-bold text-white mt-8 mb-4 border-b border-slate-800 pb-2">
      {children}
    </h1>
  ),
  h2: ({ children }) => (
    <h2
      id={toHeadingSlug(children)}
      className="text-lg sm:text-xl font-bold text-slate-100 mt-10 mb-4 pb-2 border-b border-slate-800/80 scroll-mt-20 tracking-tight flex items-center gap-2.5"
    >
      <span className="w-1 h-5 rounded-full bg-blue-500 shrink-0" />
      <span>{children}</span>
    </h2>
  ),
  h3: ({ children }) => (
    <h3
      id={toHeadingSlug(children)}
      className="text-base sm:text-[17px] font-semibold text-blue-300 mt-7 mb-2.5 scroll-mt-20 tracking-tight"
    >
      {children}
    </h3>
  ),
  h4: ({ children }) => (
    <h4
      id={toHeadingSlug(children)}
      className="text-sm sm:text-base font-semibold text-slate-200 mt-5 mb-2 scroll-mt-20"
    >
      {children}
    </h4>
  ),
  p: ({ children }) => (
    <p className="text-sm sm:text-[15px] text-slate-300 leading-[1.75] mb-4 max-w-[78ch]">
      {children}
    </p>
  ),
  strong: ({ children }) => (
    <strong className="font-semibold text-slate-100">{children}</strong>
  ),
  em: ({ children }) => (
    <em className="italic text-slate-200">{children}</em>
  ),
  a: ({ href, children }) => (
    <a
      href={href}
      target="_blank"
      rel="noreferrer"
      className="text-blue-400 hover:text-blue-300 underline underline-offset-4 decoration-blue-500/40 hover:decoration-blue-400 transition"
    >
      {children}
    </a>
  ),
  ul: ({ children }) => (
    <ul className="list-disc list-outside pl-5 sm:pl-6 space-y-2 text-sm sm:text-[15px] text-slate-300 leading-[1.7] mb-5 max-w-[78ch] marker:text-blue-400">
      {children}
    </ul>
  ),
  ol: ({ children }) => (
    <ol className="list-decimal list-outside pl-5 sm:pl-6 space-y-2 text-sm sm:text-[15px] text-slate-300 leading-[1.7] mb-5 max-w-[78ch] marker:text-blue-400 marker:font-mono marker:font-semibold">
      {children}
    </ol>
  ),
  li: ({ children }) => (
    <li className="text-sm sm:text-[15px] text-slate-300 leading-[1.7] pl-1">
      {children}
    </li>
  ),
  hr: () => <hr className="my-8 border-t border-slate-800/80" />,
  table: ({ children }) => (
    <div className="overflow-x-auto my-6 border border-slate-800/90 rounded-xl bg-slate-900/30 shadow-sm">
      <table className="w-full text-xs sm:text-[13px] text-left text-slate-300 border-collapse">
        {children}
      </table>
    </div>
  ),
  thead: ({ children }) => (
    <thead className="bg-slate-900/95 text-slate-100 font-bold border-b border-slate-700/80 uppercase text-[11px] tracking-wider font-mono">
      {children}
    </thead>
  ),
  tbody: ({ children }) => (
    <tbody className="divide-y divide-slate-800/60 [&>tr:nth-child(even)]:bg-slate-900/35 [&>tr:hover]:bg-slate-800/40">
      {children}
    </tbody>
  ),
  th: ({ children }) => (
    <th className="px-4 py-3 font-semibold whitespace-nowrap">{children}</th>
  ),
  td: ({ children }) => (
    <td className="px-4 py-3 align-top leading-relaxed">{children}</td>
  ),
  blockquote: ({ children }) => (
    <blockquote className="border-l-4 border-blue-500 pl-4 pr-4 py-3 my-5 bg-blue-950/20 border-y border-r border-y-slate-800/60 border-r-slate-800/60 rounded-r-xl text-slate-200 text-sm leading-relaxed shadow-sm [&>p:last-child]:mb-0">
      {children}
    </blockquote>
  ),
  pre: ({ children }) => (
    <PreContext.Provider value={true}>
      {children}
    </PreContext.Provider>
  ),
  code: MarkdownCode
};

interface LessonPageProps {
  lesson: LessonDetail;
  onNavigateLesson: (lessonId: string) => void;
  onLessonCompleted: (lessonId: string) => void;
}

export const LessonPage: React.FC<LessonPageProps> = React.memo(({
  lesson,
  onNavigateLesson,
  onLessonCompleted
}) => {
  const [activeTab, setActiveTab] = useState<'content' | 'simulator' | 'editor' | 'quiz' | 'challenge'>('content');
  const contentRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const scrollContainer = document.getElementById('main-scroll-container');
    if (scrollContainer) {
      scrollContainer.scrollTo({ top: 0, behavior: 'smooth' });
    } else {
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
  }, [lesson.id]);

  const renderSimulator = () => {
    switch (lesson.simulatorType) {
      case 'binary':
        return <BinarySimulator />;
      case 'stack-heap':
        return <StackHeapSimulator />;
      case 'gc':
        return <GCSimulator />;
      case 'async-flow':
        return <AsyncStateMachineSimulator />;
      case 'btree':
        return <BTreeSimulator />;
      case 'deadlock':
        return <DeadlockSimulator />;
      default:
        return null;
    }
  };

  const hasSimulator = renderSimulator() !== null;

  // Strip redundant leading "# Title" in markdownContent since LessonPage already renders the main <h1> title
  const normalizedMarkdown = React.useMemo(() => {
    if (!lesson.markdownContent) return '';
    return lesson.markdownContent.replace(/^#\s+[^\n]+\n+/, '');
  }, [lesson.markdownContent]);

  return (
    <div className="flex-1 min-w-0 w-full max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-6 sm:py-8">
      {/* Breadcrumb & Metadata */}
      <div className="flex flex-wrap items-center justify-between gap-2 text-xs text-slate-400 font-mono mb-3">
        <div className="flex items-center space-x-2">
          <span className="uppercase text-blue-400 font-bold">{lesson.levelId}</span>
          <span>/</span>
          <span>{lesson.moduleId}</span>
        </div>
        <div className="flex items-center space-x-3">
          <span className="flex items-center space-x-1">
            <Clock className="w-3.5 h-3.5 text-slate-500" />
            <span>{lesson.estimatedMinutes} Menit Baca</span>
          </span>
          <span className="px-2.5 py-0.5 rounded-md bg-blue-950/80 text-blue-300 border border-blue-900/60 font-semibold">
            {lesson.difficulty}
          </span>
        </div>
      </div>

      {/* Lesson Title */}
      <h1 className="text-2xl sm:text-3xl font-extrabold tracking-tight text-white mb-4 leading-tight">
        {lesson.title}
      </h1>

      {lesson.description && (
        <p className="text-sm sm:text-[15px] text-slate-300 leading-relaxed mb-6 border-l-3 border-blue-500 pl-4 py-0.5 bg-slate-900/30 rounded-r-lg">
          {lesson.description}
        </p>
      )}

      {/* Tabs */}
      <div className="flex items-center space-x-2 border-b border-slate-800 pb-2.5 mb-8 overflow-x-auto">
        <button
          onClick={() => setActiveTab('content')}
          className={`px-3 py-1.5 rounded-lg text-xs font-semibold flex items-center space-x-1.5 shrink-0 transition ${
            activeTab === 'content'
              ? 'bg-blue-600 text-white shadow-md shadow-blue-900/30'
              : 'text-slate-400 hover:text-white hover:bg-slate-800'
          }`}
        >
          <BookOpen className="w-3.5 h-3.5" />
          <span>Materi Teknis</span>
        </button>

        {hasSimulator && (
          <button
            onClick={() => setActiveTab('simulator')}
            className={`px-3 py-1.5 rounded-lg text-xs font-semibold flex items-center space-x-1.5 shrink-0 transition ${
              activeTab === 'simulator'
                ? 'bg-purple-600 text-white shadow-md shadow-purple-900/30'
                : 'text-purple-300 hover:text-white hover:bg-slate-800'
            }`}
          >
            <Layers className="w-3.5 h-3.5" />
            <span>Interactive Simulator</span>
          </button>
        )}

        <button
          onClick={() => setActiveTab('editor')}
          className={`px-3 py-1.5 rounded-lg text-xs font-semibold flex items-center space-x-1.5 shrink-0 transition ${
            activeTab === 'editor'
              ? 'bg-emerald-600 text-white shadow-md shadow-emerald-900/30'
              : 'text-slate-400 hover:text-white hover:bg-slate-800'
          }`}
        >
          <Sparkles className="w-3.5 h-3.5" />
          <span>Code Editor (C# Sandbox)</span>
        </button>

        {lesson.quizQuestions && lesson.quizQuestions.length > 0 && (
          <button
            onClick={() => setActiveTab('quiz')}
            className={`px-3 py-1.5 rounded-lg text-xs font-semibold flex items-center space-x-1.5 shrink-0 transition ${
              activeTab === 'quiz'
                ? 'bg-amber-600 text-white shadow-md shadow-amber-900/30'
                : 'text-slate-400 hover:text-white hover:bg-slate-800'
            }`}
          >
            <HelpCircle className="w-3.5 h-3.5" />
            <span>Kuis ({lesson.quizQuestions.length})</span>
          </button>
        )}

        {lesson.codingChallenge && (
          <button
            onClick={() => setActiveTab('challenge')}
            className={`px-3 py-1.5 rounded-lg text-xs font-semibold flex items-center space-x-1.5 shrink-0 transition ${
              activeTab === 'challenge'
                ? 'bg-red-600 text-white shadow-md shadow-red-900/30'
                : 'text-slate-400 hover:text-white hover:bg-slate-800'
            }`}
          >
            <Award className="w-3.5 h-3.5" />
            <span>Coding Challenge</span>
          </button>
        )}
      </div>

      {/* Main Tab Content */}
      {activeTab === 'content' && (
        <div className="min-w-0">
          {/* Prerequisites Banner if any */}
          {lesson.prerequisites && lesson.prerequisites.length > 0 && (
            <div className="bg-slate-900/80 border border-slate-800/90 p-4 rounded-xl mb-8 text-xs text-slate-300 shadow-sm">
              <span className="font-bold text-slate-400 uppercase tracking-wider block mb-1.5 font-mono text-[11px]">
                Prasyarat Pengetahuan:
              </span>
              <ul className="list-disc list-inside space-y-1 text-slate-300">
                {lesson.prerequisites.map((p, i) => {
                  if (typeof p === 'string') return <li key={i}>{p}</li>;
                  return (
                    <li key={i}>
                      <span
                        className="font-semibold text-blue-400 cursor-pointer hover:underline"
                        onClick={() => onNavigateLesson(p.lessonId)}
                      >
                        {p.lessonId}
                      </span>
                      {p.reason ? `: ${p.reason}` : p.context ? `: ${p.context}` : p.title ? `: ${p.title}` : ''}
                    </li>
                  );
                })}
              </ul>
            </div>
          )}

          {/* Markdown Content */}
          <div ref={contentRef} className="min-w-0 text-slate-300">
            <ReactMarkdown 
              remarkPlugins={[remarkGfm]}
              components={STATIC_MARKDOWN_COMPONENTS}
            >
              {normalizedMarkdown}
            </ReactMarkdown>
          </div>

          {/* Inline Simulator if available */}
          {lesson.simulatorType && (
            <div className="mt-10 pt-6 border-t border-slate-800">
              <div className="text-xs font-mono uppercase tracking-wider text-purple-400 font-bold mb-2">
                Simulasi Visual Konsep:
              </div>
              {renderSimulator()}
            </div>
          )}

          {/* Inline Monaco Editor */}
          <div className="mt-10 pt-6 border-t border-slate-800">
            <div className="text-xs font-mono uppercase tracking-wider text-emerald-400 font-bold mb-2">
              Latihan Coding C# Interaktif:
            </div>
            <MonacoCodeEditor
              initialCode={lesson.starterCode}
              expectedOutput={lesson.expectedOutput}
            />
          </div>

          {/* Quiz Section */}
          {lesson.quizQuestions && lesson.quizQuestions.length > 0 && (
            <div className="mt-10 pt-6 border-t border-slate-800">
              <QuizView
                lessonId={lesson.id}
                questions={lesson.quizQuestions}
                onQuizCompleted={(res) => {
                  if (res.passed) onLessonCompleted(lesson.id);
                }}
              />
            </div>
          )}

          {/* Coding Challenge Section */}
          {lesson.codingChallenge && (
            <div className="mt-10 pt-6 border-t border-slate-800">
              <ChallengeView challenge={lesson.codingChallenge} />
            </div>
          )}
        </div>
      )}

      {activeTab === 'simulator' && (
        <div>
          {renderSimulator()}
        </div>
      )}

      {activeTab === 'editor' && (
        <div>
          <MonacoCodeEditor
            initialCode={lesson.starterCode}
            expectedOutput={lesson.expectedOutput}
            height="500px"
          />
        </div>
      )}

      {activeTab === 'quiz' && lesson.quizQuestions && (
        <div>
          <QuizView
            lessonId={lesson.id}
            questions={lesson.quizQuestions}
            onQuizCompleted={(res) => {
              if (res.passed) onLessonCompleted(lesson.id);
            }}
          />
        </div>
      )}

      {activeTab === 'challenge' && lesson.codingChallenge && (
        <div>
          <ChallengeView challenge={lesson.codingChallenge} />
        </div>
      )}

      {/* Navigation Footer: Previous / Next */}
      <div className="flex items-center justify-between pt-8 mt-12 border-t border-slate-800">
        {lesson.previousLessonId ? (
          <button
            onClick={() => onNavigateLesson(lesson.previousLessonId!)}
            className="px-4 py-2 bg-slate-900 hover:bg-slate-800 border border-slate-800 text-slate-200 text-xs font-medium rounded-lg transition flex items-center space-x-2"
          >
            <ArrowLeft className="w-4 h-4" />
            <span>Materi Sebelumnya</span>
          </button>
        ) : <div />}

        {lesson.nextLessonId ? (
          <button
            onClick={() => onNavigateLesson(lesson.nextLessonId!)}
            className="px-5 py-2 bg-blue-600 hover:bg-blue-500 text-white text-xs font-medium rounded-lg shadow-lg shadow-blue-900/30 transition flex items-center space-x-2"
          >
            <span>Materi Berikutnya</span>
            <ArrowRight className="w-4 h-4" />
          </button>
        ) : (
          <div className="text-xs text-emerald-400 font-semibold flex items-center space-x-1.5">
            <CheckCircle2 className="w-4 h-4" />
            <span>Level Selesai! Selamat!</span>
          </div>
        )}
      </div>
    </div>
  );
});
