import React, { useState, useEffect } from 'react';
import { 
  Briefcase, 
  Layers, 
  Database, 
  ShieldCheck, 
  CheckCircle2, 
  Code2
} from 'lucide-react';
import { api } from '../services/api';
import { ProjectSpecification } from '../types/academy';
import { MonacoCodeEditor } from '../components/editor/MonacoCodeEditor';

export const ProjectsPage: React.FC = () => {
  const [projects, setProjects] = useState<ProjectSpecification[]>([]);
  const [selectedProject, setSelectedProject] = useState<ProjectSpecification | null>(null);
  const [loading, setLoading] = useState<boolean>(true);

  useEffect(() => {
    const fetchProjects = async () => {
      try {
        const res = await api.getProjects();
        setProjects(res);
        if (res.length > 0) setSelectedProject(res[0]);
      } catch (err) {
        console.error(err);
      } finally {
        setLoading(false);
      }
    };
    fetchProjects();
  }, []);

  if (loading) {
    return (
      <div className="flex-1 p-8 text-center text-slate-400">
        <div className="w-8 h-8 border-2 border-blue-500 border-t-transparent rounded-full animate-spin mx-auto mb-3" />
        <div>Memuat 12 Proyek Pembelajaran Enterprise...</div>
      </div>
    );
  }

  return (
    <div className="flex-1 max-w-6xl mx-auto px-6 py-8 text-slate-100">
      <div className="flex items-center space-x-3 mb-6 pb-4 border-b border-slate-800">
        <div className="p-2.5 bg-indigo-600/20 text-indigo-400 rounded-xl">
          <Briefcase className="w-6 h-6" />
        </div>
        <div>
          <h1 className="text-2xl font-bold text-white">12 Enterprise Project-Based Learning</h1>
          <p className="text-xs text-slate-400">Bangun sistem nyata dari Console CLI hingga Core Banking Backend berskala industri.</p>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-12 gap-8">
        {/* Project Selector Sidebar */}
        <div className="lg:col-span-4 space-y-2">
          <div className="text-[11px] font-mono uppercase tracking-wider text-slate-400 font-bold mb-2">
            Daftar 12 Proyek Bertingkat:
          </div>
          <div className="space-y-1.5 max-h-[700px] overflow-y-auto pr-1">
            {projects.map(proj => {
              const active = selectedProject?.id === proj.id;
              return (
                <button
                  key={proj.id}
                  onClick={() => setSelectedProject(proj)}
                  className={`w-full text-left p-3 rounded-xl border transition ${
                    active
                      ? 'bg-blue-600/20 border-blue-500 text-white shadow-md'
                      : 'bg-slate-900 border-slate-800/80 text-slate-400 hover:text-slate-200 hover:bg-slate-800/60'
                  }`}
                >
                  <div className="flex justify-between items-center mb-1">
                    <span className="font-mono text-[10px] text-blue-400 font-bold">
                      {proj.id.toUpperCase()}
                    </span>
                    <span className="text-[10px] px-1.5 py-0.5 rounded bg-slate-800 text-slate-400 font-medium">
                      {proj.difficulty}
                    </span>
                  </div>
                  <div className="text-xs font-semibold text-slate-100 truncate mb-1">
                    {proj.title.split('—')[1]?.trim() || proj.title}
                  </div>
                  <div className="text-[11px] text-slate-400 line-clamp-2 leading-relaxed">
                    {proj.subtitle}
                  </div>
                </button>
              );
            })}
          </div>
        </div>

        {/* Project Detailed Specifications View */}
        {selectedProject && (
          <div className="lg:col-span-8 space-y-6">
            <div className="bg-slate-900 border border-slate-800 rounded-2xl p-6 shadow-xl">
              <div className="flex justify-between items-start mb-2">
                <span className="text-xs font-mono font-bold text-blue-400 px-2 py-0.5 bg-blue-950/80 rounded border border-blue-900/60">
                  {selectedProject.id.toUpperCase()} • TINGKAT: {selectedProject.difficulty.toUpperCase()}
                </span>
              </div>
              <h2 className="text-xl sm:text-2xl font-bold text-white mb-2">
                {selectedProject.title}
              </h2>
              <p className="text-xs text-blue-300 font-mono mb-4">
                {selectedProject.subtitle}
              </p>
              <p className="text-sm text-slate-300 leading-relaxed mb-6 border-l-2 border-blue-500 pl-4">
                {selectedProject.overview}
              </p>

              {/* Requirements Grid */}
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-6">
                <div className="bg-slate-950 p-4 rounded-xl border border-slate-800">
                  <span className="text-xs font-bold text-blue-400 block mb-2 uppercase tracking-wider">
                    Functional Requirements:
                  </span>
                  <ul className="space-y-1.5 text-xs text-slate-300">
                    {selectedProject.functionalRequirements.map((req, i) => (
                      <li key={i} className="flex items-start space-x-2">
                        <CheckCircle2 className="w-3.5 h-3.5 text-blue-400 shrink-0 mt-0.5" />
                        <span>{req}</span>
                      </li>
                    ))}
                  </ul>
                </div>

                <div className="bg-slate-950 p-4 rounded-xl border border-slate-800">
                  <span className="text-xs font-bold text-emerald-400 block mb-2 uppercase tracking-wider">
                    Non-Functional & Quality:
                  </span>
                  <ul className="space-y-1.5 text-xs text-slate-300">
                    {selectedProject.nonFunctionalRequirements.map((req, i) => (
                      <li key={i} className="flex items-start space-x-2">
                        <ShieldCheck className="w-3.5 h-3.5 text-emerald-400 shrink-0 mt-0.5" />
                        <span>{req}</span>
                      </li>
                    ))}
                  </ul>
                </div>
              </div>

              {/* Architecture & Database */}
              <div className="space-y-4 mb-6">
                <div className="bg-slate-950 p-4 rounded-xl border border-slate-800">
                  <div className="text-xs font-bold text-purple-400 flex items-center space-x-1.5 mb-2">
                    <Layers className="w-4 h-4" />
                    <span>Desain Arsitektur Sistem:</span>
                  </div>
                  <p className="text-xs text-slate-300 leading-relaxed font-mono">
                    {selectedProject.architectureOverview}
                  </p>
                </div>

                <div className="bg-slate-950 p-4 rounded-xl border border-slate-800">
                  <div className="text-xs font-bold text-amber-400 flex items-center space-x-1.5 mb-2">
                    <Database className="w-4 h-4" />
                    <span>Skema Database & Entities:</span>
                  </div>
                  <pre className="text-xs text-slate-300 font-mono whitespace-pre-wrap bg-slate-900 p-3 rounded-lg border border-slate-800 overflow-x-auto">
                    {selectedProject.databaseSchema}
                  </pre>
                </div>
              </div>

              {/* Implementation Steps */}
              <div className="bg-slate-950 p-4 rounded-xl border border-slate-800 mb-6">
                <span className="text-xs font-bold text-slate-300 block mb-2 uppercase tracking-wider">
                  Tahapan Pengerjaan Mandiri:
                </span>
                <div className="space-y-2">
                  {selectedProject.implementationSteps.map((step, i) => (
                    <div key={i} className="text-xs text-slate-300 flex items-start space-x-2.5">
                      <span className="w-5 h-5 rounded-full bg-slate-800 border border-slate-700 font-mono text-[10px] font-bold text-slate-300 flex items-center justify-center shrink-0">
                        {i + 1}
                      </span>
                      <span className="pt-0.5">{step}</span>
                    </div>
                  ))}
                </div>
              </div>

              {/* Starter Code Editor */}
              {selectedProject.starterCode && (
                <div>
                  <div className="text-xs font-mono uppercase tracking-wider text-emerald-400 font-bold mb-2 flex items-center space-x-1.5">
                    <Code2 className="w-4 h-4" />
                    <span>Starter Repository Code:</span>
                  </div>
                  <MonacoCodeEditor
                    key={selectedProject.id}
                    initialCode={selectedProject.starterCode}
                    height="380px"
                  />
                </div>
              )}
            </div>
          </div>
        )}
      </div>
    </div>
  );
};
