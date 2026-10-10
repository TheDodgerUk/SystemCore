/*
 * Helpers for cunstom inspectors
 * 
 * 
 * 
 * 
 */ 


#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;


using UnityEditor;

namespace Helpers
{

    #region Colour
    public class Colour
    {
        public static float Colour256ToFloat(int lAmout)
        {
            return (float)lAmout / (float)256;
        }

        public static Color Colour256ToColour(int lRed, int lGreen, int lBlue)
        {
            return new Color(Colour256ToFloat(lRed), Colour256ToFloat(lGreen), Colour256ToFloat(lBlue));
        }
    }
    #endregion

    #region EditorGUILayout

    public class EditorGUILayout
    {
        public class EditorGUILayoutInfo
        {
            public int NumberOfSpaces { get; private set; }
            public int SpaceAtBegining { get; private set; }
            public int NameWidth { get; private set; }
            public int InformationWidth { get; private set; }
            public int Height { get; private set; }

            public EditorGUILayoutInfo(int lNumberOfSpaces, int lSpaceAtBegining, int lNameWidth, int lInformationWidth, int lHeight)
            {
                NumberOfSpaces = lNumberOfSpaces;
                SpaceAtBegining = lSpaceAtBegining;
                NameWidth = lNameWidth;
                InformationWidth = lInformationWidth;
                Height = lHeight;
            }
        }



        public static void ColorField(string lName, ref Color lColourVariable, EditorGUILayoutInfo lEditorGUILayoutInfo, bool lUseBeginHorizontal = true)
        {
            Space(lEditorGUILayoutInfo.NumberOfSpaces);
            if (lUseBeginHorizontal == true) UnityEditor.EditorGUILayout.BeginHorizontal();
            UnityEditor.EditorGUILayout.LabelField("", GUILayout.Width(lEditorGUILayoutInfo.SpaceAtBegining), GUILayout.Height(lEditorGUILayoutInfo.Height));
            UnityEditor.EditorGUILayout.LabelField(lName, GUILayout.Width(lEditorGUILayoutInfo.NameWidth), GUILayout.Height(lEditorGUILayoutInfo.Height));
            lColourVariable = UnityEditor.EditorGUILayout.ColorField("", lColourVariable, GUILayout.Width(lEditorGUILayoutInfo.InformationWidth), GUILayout.Height(lEditorGUILayoutInfo.Height));
            if (lUseBeginHorizontal == true) UnityEditor.EditorGUILayout.EndHorizontal();
        }


        /// <summary>
        /// eredfdtrtrfgfg
        public static void IntSlider(string lName, ref int lIntVariable, int lMin, int lMax, EditorGUILayoutInfo lEditorGUILayoutInfo, bool lUseBeginHorizontal = true)
        {
            Space(lEditorGUILayoutInfo.NumberOfSpaces);
            if (lUseBeginHorizontal == true) UnityEditor.EditorGUILayout.BeginHorizontal();
            UnityEditor.EditorGUILayout.LabelField("", GUILayout.Width(lEditorGUILayoutInfo.SpaceAtBegining), GUILayout.Height(lEditorGUILayoutInfo.Height));
            UnityEditor.EditorGUILayout.LabelField(lName, GUILayout.Width(lEditorGUILayoutInfo.NameWidth), GUILayout.Height(lEditorGUILayoutInfo.Height));
            lIntVariable = UnityEditor.EditorGUILayout.IntSlider("", lIntVariable, lMin, lMax, GUILayout.Width(lEditorGUILayoutInfo.InformationWidth), GUILayout.Height(lEditorGUILayoutInfo.Height));
            if (lUseBeginHorizontal == true) UnityEditor.EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 
        /// </summary>
        public static void LabelField(string lName, EditorGUILayoutInfo lEditorGUILayoutInfo, bool lUseBeginHorizontal = true)
        {
            Space(lEditorGUILayoutInfo.NumberOfSpaces);
            if (lUseBeginHorizontal == true) UnityEditor.EditorGUILayout.BeginHorizontal();
            UnityEditor.EditorGUILayout.LabelField("", GUILayout.Width(lEditorGUILayoutInfo.SpaceAtBegining), GUILayout.Height(lEditorGUILayoutInfo.Height));
            UnityEditor.EditorGUILayout.LabelField(lName, GUILayout.Width(lEditorGUILayoutInfo.NameWidth), GUILayout.Height(lEditorGUILayoutInfo.Height));
            if (lUseBeginHorizontal == true) UnityEditor.EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 
        /// </summary>
        public static string TextField(string lText, EditorGUILayoutInfo lEditorGUILayoutInfo, bool lUseBeginHorizontal = true)
        {

            Space(lEditorGUILayoutInfo.NumberOfSpaces);
            if (lUseBeginHorizontal == true) UnityEditor.EditorGUILayout.BeginHorizontal();
            UnityEditor.EditorGUILayout.LabelField("", GUILayout.Width(lEditorGUILayoutInfo.SpaceAtBegining), GUILayout.Height(lEditorGUILayoutInfo.Height));
            lText = UnityEditor.EditorGUILayout.TextField(lText, GUILayout.Width(lEditorGUILayoutInfo.NameWidth), GUILayout.Height(lEditorGUILayoutInfo.Height));
            if (lUseBeginHorizontal == true) UnityEditor.EditorGUILayout.EndHorizontal();
            return lText;
        }

        public static bool Button(string lText, EditorGUILayoutInfo lEditorGUILayoutInfo, bool lUseBeginHorizontal = true)
        {

            Space(lEditorGUILayoutInfo.NumberOfSpaces);
            if (lUseBeginHorizontal == true) UnityEditor.EditorGUILayout.BeginHorizontal();
            UnityEditor.EditorGUILayout.LabelField("", GUILayout.Width(lEditorGUILayoutInfo.SpaceAtBegining), GUILayout.Height(lEditorGUILayoutInfo.Height));
            bool lBool = GUILayout.Button(lText, GUILayout.Width(lEditorGUILayoutInfo.NameWidth), GUILayout.Height(lEditorGUILayoutInfo.Height));
            if (lUseBeginHorizontal == true) UnityEditor.EditorGUILayout.EndHorizontal();
            return lBool;
        }


        public static string DelayedTextField(string lName, string lTextToChange, EditorGUILayoutInfo lEditorGUILayoutInfo, bool lUseBeginHorizontal = true)
        {

            Space(lEditorGUILayoutInfo.NumberOfSpaces);
            if (lUseBeginHorizontal == true) UnityEditor.EditorGUILayout.BeginHorizontal();
            UnityEditor.EditorGUILayout.LabelField("", GUILayout.Width(lEditorGUILayoutInfo.SpaceAtBegining), GUILayout.Height(lEditorGUILayoutInfo.Height));
            UnityEditor.EditorGUILayout.LabelField(lName, GUILayout.Width(lEditorGUILayoutInfo.NameWidth), GUILayout.Height(lEditorGUILayoutInfo.Height));
            lTextToChange = UnityEditor.EditorGUILayout.TextField(lTextToChange, GUILayout.Width(lEditorGUILayoutInfo.InformationWidth), GUILayout.Height(lEditorGUILayoutInfo.Height)); //HACK should be  DelayTextField
            if (lUseBeginHorizontal == true) UnityEditor.EditorGUILayout.EndHorizontal();
            return lTextToChange;
        }



        /// <summary>
        ///  m_Consts.EDebugTypeBaseFontsChoice = (EDebugType)PopupFeildEnum("Message Type", Enum.GetNames(typeof(EDebugType)), (int)m_Consts.EDebugTypeBaseFontsChoice, 1, lSpaceAtBeginningWidth, lNameWidth, lInformationWidth);
        /// <returns></returns>
        public static int PopupEnum(string lName, string[] lList, int lIndex, EditorGUILayoutInfo lEditorGUILayoutInfo, bool lUseBeginHorizontal = true)
        {

            Space(lEditorGUILayoutInfo.NumberOfSpaces);
            if (lUseBeginHorizontal == true) UnityEditor.EditorGUILayout.BeginHorizontal();
            UnityEditor.EditorGUILayout.LabelField("", GUILayout.Width(lEditorGUILayoutInfo.SpaceAtBegining), GUILayout.Height(lEditorGUILayoutInfo.Height));
            UnityEditor.EditorGUILayout.LabelField(lName, GUILayout.Width(lEditorGUILayoutInfo.NameWidth), GUILayout.Height(lEditorGUILayoutInfo.Height));
            lIndex = UnityEditor.EditorGUILayout.Popup(lIndex, lList, GUILayout.Width(lEditorGUILayoutInfo.InformationWidth), GUILayout.Height(lEditorGUILayoutInfo.Height));
            if (lUseBeginHorizontal == true) UnityEditor.EditorGUILayout.EndHorizontal();
            return lIndex;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Popup(string lName, ref List<string> lList, ref int lIndex, EditorGUILayoutInfo lEditorGUILayoutInfo, bool lUseBeginHorizontal = true)
        {

            Space(lEditorGUILayoutInfo.NumberOfSpaces);
            UnityEditor.EditorGUILayout.BeginHorizontal();
            UnityEditor.EditorGUILayout.LabelField("", GUILayout.Width(lEditorGUILayoutInfo.SpaceAtBegining), GUILayout.Height(lEditorGUILayoutInfo.Height));
            UnityEditor.EditorGUILayout.LabelField(lName, GUILayout.Width(lEditorGUILayoutInfo.NameWidth), GUILayout.Height(lEditorGUILayoutInfo.Height));
            lIndex = UnityEditor.EditorGUILayout.Popup(lIndex, lList.ToArray<string>(), GUILayout.Width(lEditorGUILayoutInfo.InformationWidth), GUILayout.Height(lEditorGUILayoutInfo.Height));
            if (lUseBeginHorizontal == true) UnityEditor.EditorGUILayout.EndHorizontal();
        }

        public static void Toggle(string lName, ref bool lBoolVariable, EditorGUILayoutInfo lEditorGUILayoutInfo, bool lUseBeginHorizontal = true)
        {

            Space(lEditorGUILayoutInfo.NumberOfSpaces);
            if (lUseBeginHorizontal == true) UnityEditor.EditorGUILayout.BeginHorizontal();
            UnityEditor.EditorGUILayout.LabelField("", GUILayout.Width(lEditorGUILayoutInfo.SpaceAtBegining), GUILayout.Height(lEditorGUILayoutInfo.Height));
            UnityEditor.EditorGUILayout.LabelField(lName, GUILayout.Width(lEditorGUILayoutInfo.NameWidth), GUILayout.Height(lEditorGUILayoutInfo.Height));
            lBoolVariable = UnityEditor.EditorGUILayout.Toggle("", lBoolVariable, GUILayout.Width(lEditorGUILayoutInfo.InformationWidth), GUILayout.Height(lEditorGUILayoutInfo.Height));
            if (lUseBeginHorizontal == true) UnityEditor.EditorGUILayout.EndHorizontal();
        }


        public static void LabelFieldBlank(int lWidth)
        {
            UnityEditor.EditorGUILayout.LabelField("", GUILayout.Width(lWidth));
        }



        public static void Space(int lSpaceNumber)
        {
            for (int i = 0; i < lSpaceNumber; i++)
            {
                UnityEditor.EditorGUILayout.Space();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static List<TEnum> EnumList<TEnum>(string lName, ref int lFlag, EditorGUILayout.EditorGUILayoutInfo lEditorGUILayoutInfo, bool lUseBeginHorizontal = true)
        {
            string[] lStrings = System.Enum.GetNames(typeof(TEnum));
            TEnum[] lEnums = (TEnum[])System.Enum.GetValues(typeof(TEnum));
            /// <param name="lEditorGUILayoutInfo"></param>
            Space(lEditorGUILayoutInfo.NumberOfSpaces);
            if (lUseBeginHorizontal == true) UnityEditor.EditorGUILayout.BeginHorizontal();
            UnityEditor.EditorGUILayout.LabelField("", GUILayout.Width(lEditorGUILayoutInfo.SpaceAtBegining), GUILayout.Height(lEditorGUILayoutInfo.Height));
            UnityEditor.EditorGUILayout.LabelField(lName, GUILayout.Width(lEditorGUILayoutInfo.NameWidth), GUILayout.Height(lEditorGUILayoutInfo.Height));
            lFlag = UnityEditor.EditorGUILayout.MaskField("", lFlag, lStrings, GUILayout.Width(lEditorGUILayoutInfo.InformationWidth), GUILayout.Height(lEditorGUILayoutInfo.Height));
            if (lUseBeginHorizontal == true) UnityEditor.EditorGUILayout.EndHorizontal();

            List<TEnum> lSelectedOptions = new List<TEnum>();
            for (int i = 0; i < lStrings.Length; i++)
            {
                if ((lFlag & (1 << i)) == (1 << i)) lSelectedOptions.Add(lEnums[i]);
            }
            return lSelectedOptions;
        }


        /// <summary>
        /// 
        /// </summary>
        public static Dictionary<TEnum, bool> EnumDictionary<TEnum>(string lName, ref int lFlag, EditorGUILayout.EditorGUILayoutInfo lEditorGUILayoutInfo, bool lUseBeginHorizontal = true)
        {
            string[] lStrings = System.Enum.GetNames(typeof(TEnum));
            TEnum[] lEnums = (TEnum[])System.Enum.GetValues(typeof(TEnum));

            EditorGUILayout.Space(lEditorGUILayoutInfo.NumberOfSpaces);
            if (lUseBeginHorizontal == true) UnityEditor.EditorGUILayout.BeginHorizontal();
            UnityEditor.EditorGUILayout.LabelField("", GUILayout.Width(lEditorGUILayoutInfo.SpaceAtBegining), GUILayout.Height(lEditorGUILayoutInfo.Height));
            UnityEditor.EditorGUILayout.LabelField(lName, GUILayout.Width(lEditorGUILayoutInfo.NameWidth), GUILayout.Height(lEditorGUILayoutInfo.Height));
            lFlag = UnityEditor.EditorGUILayout.MaskField("", lFlag, lStrings, GUILayout.Width(lEditorGUILayoutInfo.InformationWidth), GUILayout.Height(lEditorGUILayoutInfo.Height));
            if (lUseBeginHorizontal == true) UnityEditor.EditorGUILayout.EndHorizontal();

            Dictionary<TEnum, bool> lSelectedOptions = new Dictionary<TEnum, bool>();
            for (int i = 0; i < lStrings.Length; i++)
            {
                bool lBool = ((lFlag & (1 << i)) == (1 << i));
                lSelectedOptions.Add(lEnums[i], lBool);
            }
            return lSelectedOptions;
        }

        /// <summary>
        /// Enums the dictionary.
        /// </summary>
        public static Dictionary<TEnum, bool> EnumDictionaryExcept<TEnum, TExcept>(string lName, ref int lFlag, EditorGUILayout.EditorGUILayoutInfo lEditorGUILayoutInfo, bool lUseBeginHorizontal = true)
        {

            /// ---------------------------------------------------------------
            /// 		long way of creating a list 
            /// ---------------------------------------------------------------
            TEnum[] lEnums = (TEnum[])System.Enum.GetValues(typeof(TEnum));
            TExcept[] lExcept = (TExcept[])System.Enum.GetValues(typeof(TExcept));

            List<String> lList = new List<string>();
            for (int i = 0; i < lEnums.Length; i++)
            {
                bool lValid = true;
                for (int j = 0; j < lExcept.Length; j++)
                {
                    if (lEnums[i].ToString() == lExcept[j].ToString())
                    {
                        lValid = false;
                    }
                }

                if (lValid == true)
                {
                    lList.Add(lEnums[i].ToString());
                }
            }

            string[] lStrings = lList.ToArray();


            EditorGUILayout.Space(lEditorGUILayoutInfo.NumberOfSpaces);
            if (lUseBeginHorizontal == true) UnityEditor.EditorGUILayout.BeginHorizontal();
            UnityEditor.EditorGUILayout.LabelField("", GUILayout.Width(lEditorGUILayoutInfo.SpaceAtBegining), GUILayout.Height(lEditorGUILayoutInfo.Height));
            UnityEditor.EditorGUILayout.LabelField(lName, GUILayout.Width(lEditorGUILayoutInfo.NameWidth), GUILayout.Height(lEditorGUILayoutInfo.Height));
            lFlag = UnityEditor.EditorGUILayout.MaskField("", lFlag, lStrings, GUILayout.Width(lEditorGUILayoutInfo.InformationWidth), GUILayout.Height(lEditorGUILayoutInfo.Height));
            if (lUseBeginHorizontal == true) UnityEditor.EditorGUILayout.EndHorizontal();


            // fill all enums with false     
            Dictionary<TEnum, bool> lSelectedOptions = new Dictionary<TEnum, bool>();

            foreach (TEnum lEnum in lEnums)
            {
                lSelectedOptions.Add(lEnum, false);
            }


            // look through all the strings and make the correct change
            for (int i = 0; i < lStrings.Length; i++)
            {
                TEnum lTemp = (TEnum)System.Enum.Parse(typeof(TEnum), lStrings[i]);
                bool lBool = ((lFlag & (1 << i)) == (1 << i));
                lSelectedOptions[lTemp] = lBool;
            }
            return lSelectedOptions;
        }


        #endregion
        public static int ButtonSpace(int lSpaceAtBeginng, int WidthOfButton, int NumberOfButtons, float lScreenWidth, bool lHasScrollBar)
        {
            return ButtonSpace(lSpaceAtBeginng, WidthOfButton, NumberOfButtons, (int)lScreenWidth, lHasScrollBar);
        }

        public static int ButtonSpace(int lSpaceAtBeginng, int WidthOfButton, int NumberOfButtons, int lScreenWidth, bool lHasScrollBar)
        {
            if (NumberOfButtons != 1)
            {
                int lScrollBarWidth = lHasScrollBar ? 35 : 0;
                int lTotalUsedSpace = (lSpaceAtBeginng * 2) + (WidthOfButton * NumberOfButtons) + lScrollBarWidth;
                int lSpaceLeft = lScreenWidth - lTotalUsedSpace;
                int lNumberOfSpaces = NumberOfButtons - 1;
                return lSpaceLeft / lNumberOfSpaces;
            }
            else
            {
                int lScrollBarWidth = lHasScrollBar ? 17 : 0;
                int lTotalUsedSpace = (lSpaceAtBeginng * 0) + (WidthOfButton * NumberOfButtons) + lScrollBarWidth;
                int lSpaceLeft = lScreenWidth - lTotalUsedSpace;
                return lSpaceLeft / 2;
            }


        }


        public static int ButtonSpaceCentre(int WidthOfButton, float lScreenWidth, bool lHasScrollBar)
        {
            return ButtonSpaceCentre((int)WidthOfButton, (int)lScreenWidth, lHasScrollBar);
        }

        public static int ButtonSpaceCentre(int WidthOfButton, int lScreenWidth, bool lHasScrollBar)
        {
            int lScrollBarWidth = lHasScrollBar ? 35 : 0;
            int lTotalUsedSpace = WidthOfButton + lScrollBarWidth;
            int lSpaceLeft = lScreenWidth - lTotalUsedSpace;
            int lNumberOfSpaces = 2;
            return lSpaceLeft / lNumberOfSpaces;
        }

        public static void LineRepeatField(string lString, GUIStyle lStyle)
        {
            int lRepeatTimes = (int)(300 / lString.Length);

            string lLabel = "";
            for (int i = 0; i < lRepeatTimes; i++)
            {
                lLabel += lString;
            }
#if UNITY_EDITOR
            UnityEditor.EditorGUILayout.LabelField(lLabel, lStyle);
#endif
        }


        public static void FoldOutStyleWorkAround(UnityEngine.FontStyle lSetStyle)
        {
#if UNITY_EDITOR
            GUIStyle style = UnityEditor.EditorStyles.foldout;
            style.fontStyle = lSetStyle;
#endif
        }


        public static bool CentreButton(string lText, int lWidth)
        {
            bool lReturn = false;
#if UNITY_EDITOR
            float lBlank = (UnityEditor.EditorGUIUtility.currentViewWidth / 2) - (lWidth / 2);
            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(lBlank));
            lReturn = GUILayout.Button(lText, GUILayout.Width(lWidth));
            GUILayout.EndHorizontal();
#endif
            return lReturn;
        }
    }





    public class GUISkin
    {

        public enum GUISkinName
        {
            GUISkinTick,
            GUISkinNormal,
        }

        static List<GUISkinName> m_GUISkinErrorMessage = new List<GUISkinName>();
        static Dictionary<GUISkinName, UnityEngine.GUISkin> m_GUISkin = new Dictionary<GUISkinName, UnityEngine.GUISkin>();
        public static void SetButton(GUISkinName lGUISkinName, UnityEngine.GUISkin lSkin)
        {
            if (GUISkinExists(lGUISkinName) == true)
            {
                lSkin.button = m_GUISkin[lGUISkinName].button;
            }
        }

        public static void SetButtonMiddle(GUISkinName lGUISkinName, UnityEngine.GUISkin lSkin)
        {
            if (GUISkinExists(lGUISkinName) == true)
            {
                lSkin = m_GUISkin[lGUISkinName];
            }
        }

        public static Texture GetButtonTexture(GUISkinName lGUISkinName)
        {
            if (GUISkinExists(lGUISkinName) == true)
            {
                return m_GUISkin[lGUISkinName].button.active.background;
            }
            return null;
        }

        public static Texture GetBoxTexture(GUISkinName lGUISkinName)
        {
            if (GUISkinExists(lGUISkinName) == true)
            {
                return m_GUISkin[lGUISkinName].box.active.background;
            }
            return null;
        }

        public static bool GUISkinExists(GUISkinName lGUISkinName)
        {
            if (m_GUISkin.ContainsKey(lGUISkinName) == false)
            {
                m_GUISkin.Add(lGUISkinName, Resources.Load(lGUISkinName.ToString()) as UnityEngine.GUISkin);
            }
            if (m_GUISkin[lGUISkinName] == null)
            {
                if (m_GUISkinErrorMessage.Contains(lGUISkinName) == false)
                {
                    m_GUISkinErrorMessage.Add(lGUISkinName);
                }

            }
            return m_GUISkin[lGUISkinName] != null;
        }
    }
}
#endif

