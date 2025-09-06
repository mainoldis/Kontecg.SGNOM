using DevExpress.Data.Filtering.Helpers;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraEditors.Repository;
using Kontecg.Application.Services.Dto;
using System.Linq.Expressions;
using System;
using Kontecg.Localization;
using Kontecg.Primitives;

namespace Kontecg.Presenters
{
    public class FilterColumnCollectionBuilder<TEntityDto, TPrimaryKey>  where TEntityDto : EntityDto<TPrimaryKey>
    {
        private readonly FilterColumnCollection _filterColumns;

        public FilterColumnCollectionBuilder()
        {
            _filterColumns = new FilterColumnCollection();
        }

        public FilterColumnCollectionBuilder(FilterColumnCollection filterColumns)
        {
            _filterColumns = filterColumns;
        }

        public FilterColumnCollection Build()
        {
            return _filterColumns;
        }

        public FilterColumnCollectionBuilder<TEntityDto, TPrimaryKey> AddColumn<T>(Expression<Func<TEntityDto, T>> expression,
            RepositoryItem repositoryItem = null,
            FilterColumnClauseClass clauseClass = FilterColumnClauseClass.String, string caption = null)
        {
            if (repositoryItem == null)
            {
                if (typeof(T) == typeof(bool) || (typeof(T) == typeof(bool?)))
                {
                    repositoryItem = EditorHelper.CreateEdit<RepositoryItemCheckEdit>();
                    clauseClass = FilterColumnClauseClass.Generic;
                }
                if ((typeof(T) == typeof(double)) || (typeof(T) == typeof(double?)) || (typeof(T) == typeof(decimal)) || (typeof(T) == typeof(decimal?)))
                {
                    repositoryItem = EditorHelper.CreateEdit<RepositoryItemSpinEdit>();
                    clauseClass = FilterColumnClauseClass.Generic;
                }
                if (typeof(T) == typeof(int) || (typeof(T) == typeof(int?)))
                {
                    var spinEdit = EditorHelper.CreateEdit<RepositoryItemSpinEdit>();
                    spinEdit.IsFloatValue = false;
                    repositoryItem = spinEdit;
                    clauseClass = FilterColumnClauseClass.Generic;
                }
            }
            
            _filterColumns.Add(CreateColumn(expression, caption, null, repositoryItem, clauseClass));
            return this;
        }

        public FilterColumnCollectionBuilder<TEntityDto, TPrimaryKey> AddLookupColumn<T>(Expression<Func<TEntityDto, T>> expression)
        {
            if(typeof(T) == typeof(Gender))
                return AddColumn(expression, EditorHelper.CreatePersonGenderImageComboBox(), FilterColumnClauseClass.Lookup);
            
            if (typeof(T) == typeof(EyeColor))
                return AddColumn(expression, EditorHelper.CreatePersonEyeColorImageComboBox(), FilterColumnClauseClass.Lookup);

            if (typeof(T) == typeof(Race))
                return AddColumn(expression, EditorHelper.CreatePersonRaceImageComboBox(), FilterColumnClauseClass.Lookup);
            
            return AddColumn(expression, EditorHelper.CreateEnumImageComboBox<T>(), FilterColumnClauseClass.Lookup);
        }

        public FilterColumnCollectionBuilder<TEntityDto, TPrimaryKey> AddDateTimeColumn<T>(Expression<Func<TEntityDto, T>> expression)
        {
            return AddColumn(expression, EditorHelper.CreateDateEdit(), FilterColumnClauseClass.DateTime);
        }

        private UnboundFilterColumn CreateColumn<T>(Expression<Func<TEntityDto, T>> expression, string caption, string fieldName,
            RepositoryItem repositoryItem, FilterColumnClauseClass clauseClass)
        {
            var member = (expression.Body as MemberExpression).Member;
            if (string.IsNullOrEmpty(fieldName))
                fieldName = GetFieldName<T>(expression);

            if (string.IsNullOrEmpty(caption))
                caption = GetDisplayName(member);

            return CreateColumn<T>(caption, fieldName, repositoryItem, clauseClass);
        }

        private UnboundFilterColumn CreateColumn<T>(string caption, string fieldName,
            RepositoryItem repositoryItem, FilterColumnClauseClass clauseClass)
        {
            return new UnboundFilterColumn(caption, fieldName, typeof(T), repositoryItem, clauseClass);
        }

        private string GetFieldName<T>(Expression<Func<TEntityDto, T>> expression)
        {
            var sb = new System.Text.StringBuilder();
            MemberExpression me = expression.Body as MemberExpression;
            while (me != null)
            {
                if (sb.Length > 0)
                    sb.Insert(0, ".");
                sb.Insert(0, me.Member.Name);
                me = me.Expression as MemberExpression;
            }
            return sb.ToString();
        }

        private string GetDisplayName(System.Reflection.MemberInfo member)
        {
            string displayName = member.Name;
            if (CheckDisplayNameAttribute<System.ComponentModel.DataAnnotations.DisplayAttribute>(member, a => a.GetName(), ref displayName))
                return displayName;

            if (CheckDisplayNameAttribute<System.ComponentModel.DisplayNameAttribute>(member, a => a.DisplayName, ref displayName))
                return displayName;

            if(CheckDisplayNameAttribute<KontecgDisplayNameAttribute>(member, a => a.DisplayName, ref displayName))
                return displayName;

            return displayName;
        }

        private bool CheckDisplayNameAttribute<TAttribute>(System.Reflection.MemberInfo member, Func<TAttribute, string> accessor, ref string displayName)
            where TAttribute : Attribute
        {
            var displayAttributes = member.GetCustomAttributes(typeof(TAttribute), true);
            if (displayAttributes.Length > 0)
            {
                displayName = accessor((TAttribute)displayAttributes[0]);
                return true;
            }
            return false;
        }
    }
}