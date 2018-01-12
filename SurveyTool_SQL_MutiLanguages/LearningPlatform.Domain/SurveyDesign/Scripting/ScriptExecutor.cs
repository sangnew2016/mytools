﻿using Microsoft.ClearScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Autofac;
using LearningPlatform.Domain.Common;
using Microsoft.ClearScript.V8;

namespace LearningPlatform.Domain.SurveyDesign.Scripting
{
    public class ScriptExecutor : IDisposable, IScriptExecutor
    {
        private readonly IHostObject _hostObject;
        private readonly IRequestContext _requestContext;
        private readonly IComponentContext _componentContext;
        private static readonly Regex Regex = new Regex(@"\{\{(.*?)\}\}", RegexOptions.CultureInvariant | RegexOptions.Compiled);
        private readonly V8Runtime _runtime;
        private readonly Dictionary<long, V8ScriptEngine> _engines = new Dictionary<long, V8ScriptEngine>();
        private readonly string _apiCode;


        public ScriptExecutor(IHostObject hostObject, IScriptCodeReader scriptCodeReader, IRequestContext requestContext, IComponentContext componentContext)
        {
            _hostObject = hostObject;
            _requestContext = requestContext;
            _componentContext = componentContext;
            _runtime = new V8Runtime();
            _apiCode = scriptCodeReader.GetApiCode();

        }

        public void EvaluateCode(string code)
        {
            V8ScriptEngine engine;
            if (!_engines.TryGetValue(EngineKey, out engine))
            {
                engine = CreateEngine();
            }
            engine.Evaluate(code);
        }

        public T EvaluateCode<T>(string code)
        {
            V8ScriptEngine engine;
            if (!_engines.TryGetValue(EngineKey, out engine))
            {
                engine = CreateEngine();
            }
            object o = engine.Evaluate(code);
            if (o == null || o.GetType() == typeof(Undefined))
                return default(T);
            return (T)o;
        }

        public string EvaluateString(string str)
        {
            // \u200B is zero width space
            var htmlDecodedString = HttpUtility.HtmlDecode(str.Replace("\u200B", ""));
            MatchCollection matches = Regex.Matches(htmlDecodedString);
            return matches.Cast<Match>()
                .Aggregate(htmlDecodedString,
                    (current, match) =>
                        current.Replace(match.Groups[0].Value,
                            DisplayString(EvaluateCode<object>(match.Groups[1].Value))));
        }

        public static string DisplayString(object obj)
        {
            var stringList = obj as IList<string>;
            if (stringList != null)
            {
                return string.Join(", ", stringList);
            }
            var answerAsPropertyBag = obj as PropertyBag;
            if (answerAsPropertyBag != null)
            {
                return string.Join(", ", answerAsPropertyBag.ToStringList());
            }
            return obj + "";
        }

        /*==========================================
         * instance == object duoc tao luu tru toan cuc giong bien window trong engine
         * questions == object (va duoc tao tu JavascriptPropertyBag)
         * phuong thuc, thuoc tinh se duoc render dynamic khi truy xuat tu javascript
         * Tom lai: code la javascript nhung gia tri lay tu C#
         ==========================================*/
        private V8ScriptEngine CreateEngine()
        {
            var engine = _runtime.CreateScriptEngine();
            engine.Execute(_apiCode);

            engine.AddHostObject("instance", _hostObject);
            //engine.AddHostObject("questions", new JavascriptPropertyBag(alias => new QuestionHostObject(alias, _componentContext)));
            //engine.AddHostObject("q", new JavascriptPropertyBag(alias => new QuestionHostObject(alias, _componentContext)));
            engine.AddHostObject("respondents", new JavascriptPropertyBag(key => new RespondentHostObject(key, _componentContext).GetValue()));
            engine.AddHostObject("r", new JavascriptPropertyBag(key => new RespondentHostObject(key, _componentContext).GetValue()));
            //engine.AddHostObject("loops", new JavascriptPropertyBag(alias => new LoopHostObject(alias, _componentContext)));
            //engine.AddHostObject("l", new JavascriptPropertyBag(alias => new LoopHostObject(alias, _componentContext)));

            engine.AddHostType("PropertyBag", typeof(PropertyBag));
            engine.AddHostType("List", typeof(List<string>));
            _engines[EngineKey] = engine;
            return engine;
        }

        private long EngineKey
        {
            get
            {
                //return _requestContext.Survey.Id;
                return 123;
            }
        }

        public void Dispose()
        {
            foreach (var engine in _engines.Values)
                engine.Dispose();
            _runtime.Dispose();
        }
    }
}
