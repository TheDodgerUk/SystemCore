/* Created by Anthony Brown 
 * 
 * This allows you to change all the elements in ConsoleExtra
 * 
 * 
 * 
 */



#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class Settings : EditorWindow
{
    private Vector2        m_ScrollPosTab = Vector2.zero;
    private Logging        m_Logging;
    public static Settings m_Window = null;


    public  Settings(Logging lLogging)
	{
		m_Window         = (Settings)EditorWindow.GetWindow(typeof(Settings), true, "Settings Menu");
		m_Window.maxSize = new Vector2(215f * 2.2f, 110.0f * 5.0f);
		m_Window.minSize = m_Window.maxSize;
        m_Logging        = lLogging;
    }

	private void OnGUI()
	{
		OnInspectorUpdate();
		FillInWindowSetting();
		Repaint();
		Logging.m_Repaint = true;
	}

    /// <summary>
    /// 
    /// </summary>
    private void OnInspectorUpdate()
	{
		if (null == m_Window)
		{
			m_Window = (Settings)EditorWindow.GetWindow(typeof(Settings), true, "Settings Menu");
		}

		Repaint();
		Logging.m_Repaint = true;
	}

    /// <summary>
    /// 
    /// </summary>
    private void FillInWindowSetting()
	{
		if((null == m_Logging)  || (null == m_Logging.m_SetupData))
		{
			this.Close();
		}

		m_ScrollPosTab      = EditorGUILayout.BeginScrollView(m_ScrollPosTab, true, false);
		GUI.backgroundColor = Color.white;
		
		Helpers.EditorGUILayout.Space(3);
        int lInt = Helpers.EditorGUILayout.PopupEnum("Message Type", Enum.GetNames(typeof(ConsoleExtraEnum.EDebugTypeBase)), (int)m_Logging.m_SetupData.m_EDebugTypeBaseFontsChoice, m_Logging.m_SetupData.m_GuiInfo1);
        m_Logging.m_SetupData.m_EDebugTypeBaseFontsChoice = (ConsoleExtraEnum.EDebugTypeBase)lInt;

        //--------------------------------------------------
        //      Safety Check
        //--------------------------------------------------
        if (m_Logging.m_SetupData.m_EDebugTypeBaseFontDictionary.ContainsKey(m_Logging.m_SetupData.m_EDebugTypeBaseFontsChoice) == false)
		{
            m_Logging.m_SetupData.m_EDebugTypeBaseFontDictionary.Add(m_Logging.m_SetupData.m_EDebugTypeBaseFontsChoice, new Logging.EDebugTypeBaseFont());
		}
		
		//--------------------------------------------------------
		//          Messages
		//--------------------------------------------------------
		Logging.FontData lMessage = m_Logging.m_SetupData.m_EDebugTypeBaseFontDictionary[m_Logging.m_SetupData.m_EDebugTypeBaseFontsChoice].m_Message;
		
		Helpers.EditorGUILayout.LabelField("Message", m_Logging.m_SetupData.m_GuiInfo2);
		Helpers.EditorGUILayout.Popup("Message Font",  ref m_Logging.m_SetupData.m_FontNameList, ref lMessage.m_FontIndex, m_Logging.m_SetupData.m_GuiInfo3);
		
		//----------------------------------------------
		//          Safety Checks
		//----------------------------------------------
		if (lMessage.m_FontIndex < m_Logging.m_SetupData.m_FontArray.Length)
		{
			lMessage.m_Font = m_Logging.m_SetupData.m_FontArray[lMessage.m_FontIndex];
		}
		
		Helpers.EditorGUILayout.IntSlider ("Message Font Size",   ref lMessage.m_FontSize , 3, 20, m_Logging.m_SetupData.m_GuiInfo3);
		Helpers.EditorGUILayout.ColorField("Message Font Colour", ref lMessage.m_Colour, m_Logging.m_SetupData.m_GuiInfo3);
		lMessage.m_FontStyle = (FontStyle)Helpers.EditorGUILayout.PopupEnum("Message Font Style", Enum.GetNames(typeof(FontStyle)), (int)lMessage.m_FontStyle, m_Logging.m_SetupData.m_GuiInfo3);
		//-----------------------------------------------------------------
		
		//--------------------------------------------------------
		//          lCallStack
		//--------------------------------------------------------
		{
			Logging.FontData lCallStack = m_Logging.m_SetupData.m_EDebugTypeBaseFontDictionary[m_Logging.m_SetupData.m_EDebugTypeBaseFontsChoice].m_CallStack;
			
			Helpers.EditorGUILayout.LabelField  ("CallStack", m_Logging.m_SetupData.m_GuiInfo2);
			Helpers.EditorGUILayout.Popup       ("CallStack Font",            ref m_Logging.m_SetupData.m_FontNameList, ref lCallStack.m_FontIndex, m_Logging.m_SetupData.m_GuiInfo3);
			Helpers.EditorGUILayout.IntSlider   ("CallStack Font Size", ref lCallStack.m_FontSize,  3, 20, m_Logging.m_SetupData.m_GuiInfo3);
			Helpers.EditorGUILayout.ColorField  ("CallStack Font Colour", ref lCallStack.m_Colour, m_Logging.m_SetupData.m_GuiInfo3);
			lCallStack.m_FontStyle = (FontStyle)Helpers.EditorGUILayout.PopupEnum("CallStack Font Style", Enum.GetNames(typeof(FontStyle)), (int)lCallStack.m_FontStyle, m_Logging.m_SetupData.m_GuiInfo3);
		}
		
		Helpers.EditorGUILayout.Space(5);
		Helpers.EditorGUILayout.LabelField("Tab", m_Logging.m_SetupData.m_GuiInfo1);
		Helpers.EditorGUILayout.Popup     ("Tab Fonts", ref m_Logging.m_SetupData.m_FontNameList, ref m_Logging.m_SetupData.m_TabFont.m_FontIndex, m_Logging.m_SetupData.m_GuiInfo2);
		
		//--------------------------------------------------
		//      Safety Check
		//--------------------------------------------------
		if (m_Logging.m_SetupData.m_FontArray != null && m_Logging.m_SetupData.m_TabFont.m_FontIndex < m_Logging.m_SetupData.m_FontArray.Length)
		{
            m_Logging.m_SetupData.m_TabFont.m_Font = m_Logging.m_SetupData.m_FontArray[m_Logging.m_SetupData.m_TabFont.m_FontIndex];       
		}
		
		
		Helpers.EditorGUILayout.IntSlider("Tab Font Size", ref m_Logging.m_SetupData.m_TabFont.m_FontSize, 2, 20, m_Logging.m_SetupData.m_GuiInfo2);
		
		Logging.FontData lFontData       = m_Logging.m_SetupData.m_TabStyleDictionary[m_Logging.m_SetupData.m_TabStyleIndex];
        m_Logging.m_SetupData.m_TabStyleIndex = (Logging.ETabStyle)Helpers.EditorGUILayout.PopupEnum("Tab Type", Enum.GetNames(typeof(Logging.ETabStyle)), (int)m_Logging.m_SetupData.m_TabStyleIndex, m_Logging.m_SetupData.m_GuiInfo2);
		lFontData.m_FontStyle              = (FontStyle)Helpers.EditorGUILayout.PopupEnum("Font Style", Enum.GetNames(typeof(FontStyle)), (int)lFontData.m_FontStyle, m_Logging.m_SetupData.m_GuiInfo3);
		Helpers.EditorGUILayout.ColorField("Font Colour", ref lFontData.m_Colour, m_Logging.m_SetupData.m_GuiInfo3);
		
		Helpers.EditorGUILayout.Space(4);
		
		Helpers.EditorGUILayout.Toggle("SelectEmptyItem", ref m_Logging.m_SetupData.m_SelectEmptyItem, m_Logging.m_SetupData.m_GuiInfo1);
        m_Logging.m_SetupData.m_DisplayTime = (Logging.EDisplayTime)Helpers.EditorGUILayout.PopupEnum("Display Time", Enum.GetNames(typeof(Logging.EDisplayTime)), (int)m_Logging.m_SetupData.m_DisplayTime, m_Logging.m_SetupData.m_GuiInfo1);
		
		Helpers.EditorGUILayout.Space(1);
		//----------------------------------------------------------------------
		Helpers.EditorGUILayout.Space(4);

        EditorGUILayout.BeginHorizontal();
        int lMiddleWidth = Helpers.EditorGUILayout.ButtonSpace(m_Logging.m_SetupData.m_ButtonStartWidth, m_Logging.m_SetupData.m_ButtonWidth, 1, position.width, true);
        Helpers.EditorGUILayout.LabelFieldBlank(lMiddleWidth);

        EditorGUILayout.EndHorizontal();
        Helpers.EditorGUILayout.Space(2);
        EditorGUILayout.BeginHorizontal();
		{
			lMiddleWidth        = Helpers.EditorGUILayout.ButtonSpace(m_Logging.m_SetupData.m_ButtonStartWidth, m_Logging.m_SetupData.m_ButtonWidth, 2, position.width, true);
			
			EditorGUILayout.LabelField("", GUILayout.Width(m_Logging.m_SetupData.m_ButtonStartWidth));
			if (GUILayout.Button      ("Save!", GUILayout.Width(m_Logging.m_SetupData.m_ButtonWidth), GUILayout.Height(m_Logging.m_SetupData.m_ButtonHeight)))
			{
                m_Logging.Save();
				Repaint();
			}
			
			Helpers.EditorGUILayout.LabelFieldBlank(lMiddleWidth);
			
			if (GUILayout.Button("Revert!", GUILayout.Width(m_Logging.m_SetupData.m_ButtonWidth), GUILayout.Height(m_Logging.m_SetupData.m_ButtonHeight)))
			{
                m_Logging.Load();
				Repaint();
			}
		}
		EditorGUILayout.EndHorizontal();		
		Helpers.EditorGUILayout.Space(3);
		EditorGUILayout.EndScrollView();
	}
}
#endif
