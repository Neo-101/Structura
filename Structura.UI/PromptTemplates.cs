using System.Collections.Generic;
using System.Globalization;

namespace Structura.UI
{
    public static class PromptTemplates
    {
        private static readonly Dictionary<string, string> _prompts = new Dictionary<string, string>
        {
            { "en", "I will provide you with the structure JSON of my Obsidian knowledge base. Please act as a senior knowledge management expert to analyze my directory structure for issues such as: 1. Classification redundancy 2. Search difficulty 3. Naming conflicts 4. Chaotic attachment management, and provide specific restructuring plans." },
            { "zh", "我将为你提供我 Obsidian 知识库的结构 JSON。请扮演一名资深的知识管理专家，分析我的目录结构是否存在：1. 归类冗余 2. 搜索困难 3. 命名冲突 4. 附件管理混乱 等问题，并给出具体的重组方案。" },
            { "ja", "Obsidianナレッジベースの構造JSONを提供します。シニアナレッジ管理エキスパートとして、私のディレクトリ構造に以下の問題がないか分析してください：1. 分類の重複 2. 検索の難しさ 3. 命名の競合 4. 添付ファイル管理の混乱。そして、具体的な再構築案を提示してください。" },
            { "es", "Le proporcionaré el JSON de estructura de mi base de conocimientos Obsidian. Por favor, actúe como un experto senior en gestión del conocimiento para analizar mi estructura de directorios en busca de problemas como: 1. Redundancia de clasificación 2. Dificultad de búsqueda 3. Conflictos de nomenclatura 4. Gestión caótica de archivos adjuntos, y proporcione planes de reestructuración específicos." },
            { "fr", "Je vais vous fournir le JSON de structure de ma base de connaissances Obsidian. Veuillez agir en tant qu'expert senior en gestion des connaissances pour analyser la structure de mes répertoires et identifier des problèmes tels que : 1. Redondance de classification 2. Difficulté de recherche 3. Conflits de nommage 4. Gestion chaotique des pièces jointes, et fournir des plans de restructuration spécifiques." }
        };

        public static string GetPrompt()
        {
            var culture = CultureInfo.CurrentCulture;
            // Match specific culture or fallback to two-letter ISO code
            if (_prompts.ContainsKey(culture.Name)) return _prompts[culture.Name];
            if (_prompts.ContainsKey(culture.TwoLetterISOLanguageName)) return _prompts[culture.TwoLetterISOLanguageName];
            
            // Default to English
            return _prompts["en"];
        }

        public static string GetPrompt(string langCode)
        {
             if (_prompts.ContainsKey(langCode)) return _prompts[langCode];
             return _prompts["en"];
        }
    }
}
