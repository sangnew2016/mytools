﻿using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace LearningPlatform.Domain.SurveyDesign.LangageStrings
{
    public class LanguageString : ILanguageString
    {
        public LanguageString()
        {
            Items = new List<LanguageStringItem>();
        }

        public long Id { get; set; }
        public long? SurveyId { get; set; }
        public List<LanguageStringItem> Items { get; set; }

        public ILanguageStringItem GetItem(string language)
        {
            var cultureInfoName = language;
            while (!string.IsNullOrEmpty(cultureInfoName))
            {
                var item = Items.FirstOrDefault(o => o.Language == cultureInfoName);
                if (item != null)
                {
                    return item;
                }

                var culture = CultureInfo.GetCultureInfo(cultureInfoName);
                cultureInfoName = culture.Parent.Name;
            }
            return null;
        }

        public void AddItem(ILanguageStringItem languageStringItem)
        {
            Items.Add((LanguageStringItem)languageStringItem);
        }

        public ILanguageStringItem FirstItem()
        {
            return Items.FirstOrDefault();
        }
    }
}
