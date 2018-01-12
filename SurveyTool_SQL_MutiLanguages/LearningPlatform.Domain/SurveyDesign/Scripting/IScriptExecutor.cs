﻿using System;

namespace LearningPlatform.Domain.SurveyDesign.Scripting
{
    public interface IScriptExecutor : IDisposable
    {
        T EvaluateCode<T>(string code);
        void EvaluateCode(string code);
        string EvaluateString(string str);
    }
}
