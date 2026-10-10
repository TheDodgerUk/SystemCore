
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System;

// Simple script that creates a new non-dockable window

public class SearchMenu: EditorWindow
{
    //---------------------------------------------------------------
    // these are the enums that you can use when using ConsoleExtra.Log()
    //---------------------------------------------------------------
    public enum ESearchType
    {
        Message,
        CallStack,
		GameObjectName,
		All,
    }


    static private SearchMenu m_Window = null;
    static private bool m_bLoadVariablesOnStart = false;
    public SetupData  m_SetupData = null;




    [System.Serializable]
    public class SetupData
    {
        public string m_SearchText = "";

        public ESearchType SearchType = ESearchType.Message;

        public int m_iSpaceAtBeginning   = 50;
        public int m_iNameWidth          = 150;
        public int m_iInformationWidth   = 150;
        public int  m_iInformationHeight = 20;

        public Helpers.EditorGUILayout.EditorGUILayoutInfo m_GuiInfo0 = null;
        public Helpers.EditorGUILayout.EditorGUILayoutInfo m_GuiInfo1 = null;
        public Helpers.EditorGUILayout.EditorGUILayoutInfo m_GuiInfo2 = null;
        public Helpers.EditorGUILayout.EditorGUILayoutInfo m_GuiInfo3 = null;
        public Helpers.EditorGUILayout.EditorGUILayoutInfo m_GuiInfo4 = null;
        public Helpers.EditorGUILayout.EditorGUILayoutInfo m_GuiInfo5 = null;
    }




    //--------------------------------------------------
    // these are what apear in the console its self
    //--------------------------------------------------




    public enum ETabStyle
    {
        HighLight,
        Normal,
        NewItem,
        NoNewItem,
    }

    public enum EDisplayTime
    {
        NoTime,
        FromStart,
        RealTime,
        RealTimeShort,
    }

    public enum EDisplayGameObjectName
    {
        NoName,
        Name,
        FullName,
    }



    Logging m_Logging;
    public  SearchMenu(Logging lLogging)
    {
        m_Window                 = (SearchMenu)EditorWindow.GetWindow(typeof(SearchMenu), true, "Search Menu");
        m_Window.maxSize         = new Vector2(215f * 2, 110f * 2);
        m_Window.minSize         = m_Window.maxSize;
        m_bLoadVariablesOnStart  = false;
        m_Logging                = lLogging;
        m_SetupData              = new SetupData();
        m_SetupData.m_SearchText = string.Empty;
        m_bLoadVariablesOnStart  = false;
    }

    /// <summary>
    /// 
    /// </summary>
    static void Save()
    {
        // place holder
    }


    /// <summary>
    /// 
    /// </summary>
    static void Load()
    {
        // place holder
    }


    /// <summary>
    /// 
    /// </summary>
    private void OnGUI()
    {
		if(null == m_Logging)
		{
			this.Close();
        }
        if (null != m_SetupData)
        {
            OnInspectorUpdate();
            if (true == m_bLoadVariablesOnStart)
            {
                FillInWindowSetting();
            }
        }
        Repaint();
		Logging.m_Repaint = true;
    }


    /// <summary>
    /// 
    /// </summary>
    void OnInspectorUpdate()
    {
		if(null == m_Logging)
		{
			this.Close();
        }
        if (null == m_Window)
        {
            m_Window = (SearchMenu)EditorWindow.GetWindow(typeof(SearchMenu), false, "Console Extra");
        }

        if (false == m_bLoadVariablesOnStart)
        {
            InitialiseGuiFormat();
            Load();
            m_bLoadVariablesOnStart = true;
        }
        Repaint();
		Logging.m_Repaint = true;
    }


    /// <summary>
    /// 
    /// <returns></returns>

    void InitialiseGuiFormat()
    {
        m_SetupData.m_GuiInfo0 = new Helpers.EditorGUILayout.EditorGUILayoutInfo(0, m_SetupData.m_iSpaceAtBeginning * 0, 0, 10, m_SetupData.m_iInformationHeight);
        m_SetupData.m_GuiInfo1 = new Helpers.EditorGUILayout.EditorGUILayoutInfo(1, m_SetupData.m_iSpaceAtBeginning * 0, m_SetupData.m_iNameWidth, m_SetupData.m_iInformationWidth, m_SetupData.m_iInformationHeight);
        m_SetupData.m_GuiInfo2 = new Helpers.EditorGUILayout.EditorGUILayoutInfo(1, m_SetupData.m_iSpaceAtBeginning * 1, m_SetupData.m_iNameWidth, m_SetupData.m_iInformationWidth, m_SetupData.m_iInformationHeight);
        m_SetupData.m_GuiInfo3 = new Helpers.EditorGUILayout.EditorGUILayoutInfo(1, m_SetupData.m_iSpaceAtBeginning * 2, m_SetupData.m_iNameWidth, m_SetupData.m_iInformationWidth, m_SetupData.m_iInformationHeight);
        m_SetupData.m_GuiInfo4 = new Helpers.EditorGUILayout.EditorGUILayoutInfo(1, m_SetupData.m_iSpaceAtBeginning * 3, m_SetupData.m_iNameWidth, m_SetupData.m_iInformationWidth, m_SetupData.m_iInformationHeight);
        m_SetupData.m_GuiInfo5 = new Helpers.EditorGUILayout.EditorGUILayoutInfo(1, m_SetupData.m_iSpaceAtBeginning * 4, m_SetupData.m_iNameWidth, m_SetupData.m_iInformationWidth, m_SetupData.m_iInformationHeight);
    }


    /// <summary>
    /// 
    /// </summary>
    void FillInWindowSetting()
    {
		if(null == m_Logging)
		{
			this.Close();
        }
        GUI.backgroundColor = Color.white;

        Helpers.EditorGUILayout.Space(3);

        m_SetupData.SearchType   = (ESearchType)Helpers.EditorGUILayout.PopupEnum("Message Type", Enum.GetNames(typeof(ESearchType)), (int)m_SetupData.SearchType, m_SetupData.m_GuiInfo1);
        m_SetupData.m_SearchText = Helpers.EditorGUILayout.DelayedTextField("String" , m_SetupData.m_SearchText, m_SetupData.m_GuiInfo1);
    }


}
#endif

