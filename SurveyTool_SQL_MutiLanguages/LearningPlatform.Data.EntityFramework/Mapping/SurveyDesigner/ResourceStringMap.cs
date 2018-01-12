﻿using LearningPlatform.Domain.SurveyDesign.Resources;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace LearningPlatform.Data.EntityFramework.Mapping.SurveyDesigner
{
    internal class ResourceStringMap : EntityTypeConfiguration<ResourceString>
    {
        public ResourceStringMap()
        {
            Property(t => t.Name)
                .IsRequired()
                .IsVariableLength()
                .HasMaxLength(30)
                .HasColumnAnnotation("Index",
                    new IndexAnnotation(new IndexAttribute("ix_ResourceString_Name")));

            HasMany(k => k.Items)
                .WithRequired()
                .HasForeignKey(k => k.ResourceStringId);

            ToTable("Resources");
        }
    }
}
