using System;
using System.Drawing;
using DevExpress.Utils.Svg;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraLayout;
using Kontecg.Extensions;
using Kontecg.Primitives;
using Kontecg.UI.Inputs;
using Kontecg.CustomInputTypes;

namespace Kontecg.Presenters
{
    public static class EditorHelper
    {
        public static RepositoryItemImageComboBox CreateEnumImageComboBox<TEnum>(
            DevExpress.XtraEditors.Container.EditorContainer container,
            Converter<TEnum, string> displayTextConverter = null)
        {
            return CreateEdit<RepositoryItemImageComboBox>((RepositoryItemImageComboBox)null, container?.RepositoryItems,
                (e) => e.Items.AddEnum<TEnum>(displayTextConverter));
        }

        public static RepositoryItemImageComboBox CreateEnumImageComboBox<TEnum>(
            RepositoryItemImageComboBox edit = null, RepositoryItemCollection collection = null,
            Converter<TEnum, string> displayTextConverter = null)
        {
            return CreateEdit<RepositoryItemImageComboBox>(edit, collection,
                (e) => e.Items.AddEnum<TEnum>(displayTextConverter));
        }

        public static RepositoryItemDateEdit CreateDateEdit(RepositoryItemDateEdit edit = null, RepositoryItemCollection collection = null)
        {
            return CreateEdit(edit, collection);
        }

        public static RepositoryItem CreateEdit<TEdit>(InputTypeBase inputType, RepositoryItemCollection collection = null)
        {
            return inputType switch
                   {
                       CheckboxInputType => CreateEdit<RepositoryItemCheckEdit>(inputType,
                           (RepositoryItemCheckEdit) null, collection,
                           (edit, type) =>
                           {
                               
                           }),
                       ComboboxInputType => CreateEdit<RepositoryItemImageComboBox>(inputType,
                           (RepositoryItemImageComboBox) null, collection, (edit, type) =>
                           {

                           }),
                       SingleLineStringInputType => CreateEdit<RepositoryItemTextEdit>(inputType,
                           (RepositoryItemTextEdit) null, collection, (edit, type) =>
                           {

                           }),
                       MultiSelectComboboxInputType => CreateEdit<RepositoryItemLookUpEdit>(inputType,
                           (RepositoryItemLookUpEdit) null, collection, (edit, type) =>
                           {

                           }),
                       _ => null
                   };
        }

        private static TEdit CreateEdit<TEdit>(InputTypeBase inputType, TEdit edit = null, RepositoryItemCollection collection = null, Action<TEdit, IInputType> initialize = null)
            where TEdit : RepositoryItem, new()
        {
            edit = edit ?? new TEdit();
            collection?.Add(edit);
            initialize?.Invoke(edit, inputType);
            return edit;
        }

        public static TEdit CreateEdit<TEdit>(TEdit edit = null, RepositoryItemCollection collection = null, Action<TEdit> initialize = null)
            where TEdit : RepositoryItem, new()
        {
            edit = edit ?? new TEdit();
            collection?.Add(edit);
            initialize?.Invoke(edit);
            return edit;
        }

        public static RepositoryItemImageComboBox CreatePersonGenderImageComboBox(RepositoryItemImageComboBox edit = null, RepositoryItemCollection collection = null)
        {
            RepositoryItemImageComboBox ret = CreateEnumImageComboBox<Gender>(edit, collection, input =>
            {
                return input switch
                       {
                           Gender.F => L("GenderFemale"),
                           Gender.M => L("GenderMale"),
                           Gender.O => L("GenderOther"),
                           _ => throw new ArgumentOutOfRangeException(nameof(input), input, null)
                       };
            });
            ret.SmallImages = CreatePersonGenderImageCollection();
            if (edit == null)
                ret.GlyphAlignment = HorzAlignment.Center;
            return ret;
        }

        public static RepositoryItemImageComboBox CreatePersonRaceImageComboBox(RepositoryItemImageComboBox edit = null, RepositoryItemCollection collection = null)
        {
            RepositoryItemImageComboBox ret = CreateEnumImageComboBox<Race>(edit, collection, input =>
            {
                return input switch
                       {
                           Race.B => L("SkinWhite"),
                           Race.M => L("SkinMixed"),
                           Race.N => L("SkinBlack"),
                           Race.O => L("SkinOther"),
                           _ => throw new ArgumentOutOfRangeException(nameof(input), input, null)
                       };
            });
            //ret.SmallImages = CreatePersonGenderImageCollection();
            //if (edit == null)
            //    ret.GlyphAlignment = HorzAlignment.Center;
            return ret;
        }

        public static RepositoryItemImageComboBox CreatePersonEyeColorImageComboBox(RepositoryItemImageComboBox edit = null, RepositoryItemCollection collection = null)
        {
            RepositoryItemImageComboBox ret = CreateEnumImageComboBox<EyeColor>(edit, collection, input =>
            {
                return input switch
                       {
                           EyeColor.Black => L("EyeColorBlack"),
                           EyeColor.Brown => L("EyeColorBrown"),
                           EyeColor.Blue => L("EyeColorBlue"),
                           EyeColor.Green => L("EyeColorGreen"),
                           EyeColor.Gray => L("EyeColorGray"),
                           EyeColor.Hazelnut => L("EyeColorHazelnut"),
                           EyeColor.Other => L("EyeColorOther"),
                           _ => throw new ArgumentOutOfRangeException(nameof(input), input, null)
                       };
            });
            //ret.SmallImages = CreatePersonGenderImageCollection();
            //if (edit == null)
            //    ret.GlyphAlignment = HorzAlignment.Center;
            return ret;
        }

        private static SvgImageCollection CreatePersonGenderImageCollection()
        {
            SvgImageCollection svgImageCollection = new();
            svgImageCollection.ImageSize = new Size(16, 16);
            svgImageCollection.Add(SvgImage.FromResources("Kontecg.Resources." + "Miss.svg", typeof(KontecgWinFormsModule).GetAssembly()));
            svgImageCollection.Add(SvgImage.FromResources("Kontecg.Resources." + "Mr.svg", typeof(KontecgWinFormsModule).GetAssembly()));
            return svgImageCollection;
        }

        public static void ApplyBindingSettings<TEntity>(BaseEdit edit, LayoutControl layoutControl)
        {
            var memberInfo = edit.DataBindings["EditValue"]?.BindingMemberInfo.BindingMember;

            if(memberInfo.IsNullOrEmpty()) return;

            if (DataAnnotationHelper.IsRequired<TEntity>(memberInfo))
            {
                if (layoutControl != null)
                {
                    var itemForEdit = layoutControl.GetItemByControl(edit);
                    itemForEdit.AllowHtmlStringInCaption = true;
                    itemForEdit.Text += @" <color=red>*</color>";
                }
            }

            if (edit is TextEdit textEdit)
            {
                if (DataAnnotationHelper.IsPhone<TEntity>(memberInfo))
                {
                    textEdit.Properties.Mask.MaskType = MaskType.Simple;
                    textEdit.Properties.Mask.EditMask = @"(999) 000-0000";
                    textEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
                }
            }
        }

        private static string L(string name)
        {
            return Localization.LocalizationHelper.GetString(KontecgCoreConsts.LocalizationSourceName, name);
        }
    }
}
