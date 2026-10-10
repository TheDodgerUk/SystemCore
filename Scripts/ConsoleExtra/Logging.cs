/* Created by Anthony Brown 
 * 
 * handles all the inputs 
 */


using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using ConsoleExtraEnum;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public class LoggingExtra 
{
    public const string AUTO_SHOW_CONSOLELOG = "AUTO_SHOW_CONSOLELOG";
    static Logging m_ConsoleInstance;
    public static void Log(string message, GameObject gameObjectRef, ConsoleExtraEnum.EDebugType debugType)
    {
        if (object.ReferenceEquals(m_ConsoleInstance,null))
        {
            m_ConsoleInstance = (Logging)ScriptableObject.CreateInstance(typeof(Logging));
            m_ConsoleInstance.m_SetupData = new Logging.SetupData();

            m_ConsoleInstance.CreateListOfFonts();
            m_ConsoleInstance.InitialiseEDebugTypeBaseFonts();
            m_ConsoleInstance.InitialiseTabTyle();
            m_ConsoleInstance.Load();
            m_ConsoleInstance.InitialiseGuiFormat();
        }

        m_ConsoleInstance.AddToMessages(m_ConsoleInstance, message, debugType, gameObjectRef);
        Logging.m_Repaint = true;
    }
}



[InitializeOnLoad]
public class Logging: EditorWindow
{
	
    #region varaibles
    static string m_LoggingString = "Logging_";

    static Logging        m_ConsoleInstance;
    public static Logging m_Window                = null;
    static EDebugTypeBase m_EDebugTypeBase        = EDebugTypeBase.All;
    internal SetupData    m_SetupData                = null;
    static bool           m_LoadVariablesOnStart  = false;
    public static bool    m_Repaint               = true;
	
    static SimpleDictionaryList<EDebugTypeBase, Data> m_DictionaryListData = new SimpleDictionaryList<EDebugTypeBase, Data>();
    static Dictionary<EDebugTypeBase, int> m_DictionaryListLastViewedCount  = new Dictionary<EDebugTypeBase, int>();

	static SimpleDictionaryList<EDebugTypeBase, Data> m_DictionaryListCountData = new SimpleDictionaryList<EDebugTypeBase, Data>();




	static void AddDictionaryListCountData(EDebugTypeBase debugTypeBase, Data newData)
	{
		bool found      = false;
		List<Data> dataList = m_DictionaryListCountData.GetList(debugTypeBase);
		Data dataItem       = null;
		for(int i = 0; i < dataList.Count; i++)
		{
			dataItem = dataList[i];
			if(dataItem.Equals(newData) == true)
			{
				found = true;
				break;
			}
		}

		if(found == true)
		{
			dataItem.m_Count++;
		}
		else
		{
			dataList.Add(newData);
            dataList[dataList.Count - 1].m_Count = 1;
		}
	}

	//
    [System.Serializable]
    internal class SetupData
    {
		public bool m_AudioTracking        = false;
        public Data m_SelectedItem         = null;
		public bool m_Compressed           = false;
        public char m_LineSplitter         = '\n';
        public string m_ClearButtonText    = "Clear Data";
		public Color m_StoreColour         = Helpers.Colour.Colour256ToColour(11, 196, 89); // nice green
        public string m_CSC_Directory      = @"C:\Windows\Microsoft.NET\Framework64\v3.5\csc.exe";
        public string m_STAY_CSC_Directory = @"C:\Windows\Microsoft.NET\Framework64\v3.5\csc.exe";
        public bool m_SelectEmptyItem      = false;
        public int m_StartSpace            = 10;


        // ----------------------------------------------------------
        //          Font Data
        // ----------------------------------------------------------
        public Font[] m_FontArray          = null;
        public List<string> m_FontNameList = new List<string>();
        public FontData m_TabFont          = new FontData(Color.black);
        public Dictionary<ETabStyle, FontData> m_TabStyleDictionary = new Dictionary<ETabStyle, FontData>();


        public ETabStyle m_TabStyleIndex              = ETabStyle.Normal;

        public EDebugTypeBase m_EDebugTypeBaseFontsChoice = EDebugTypeBase.All;
		public Dictionary<EDebugTypeBase, EDebugTypeBaseFont> m_EDebugTypeBaseFontDictionary = new Dictionary<EDebugTypeBase, EDebugTypeBaseFont>();

		public Vector2 m_ScrollPos    = Vector2.zero;
		public Vector2 m_ScrollPosTab = Vector2.zero;
        public Vector2 m_ScrollPosCallStack = Vector2.zero;
        public int m_EnumAllFlag = int.MaxValue;
        public Dictionary<EDebugTypeBase, bool> m_EDebugTypeDictionary = new Dictionary<EDebugTypeBase, bool>();

        public EDisplayTime m_DisplayTime = EDisplayTime.FromStart;


        public int m_ButtonHeight     = 50;
        public int m_ButtonStartWidth = 25;
        public int m_ButtonWidth      = 150;

        public int m_CurrentIndex = 1;



		public int m_SpaceAtBeginning  = 50;
		public int m_NameWidth         = 150;
		public int m_InformationWidth  = 150;
        public int m_InformationHeight = 25;

        public Helpers.EditorGUILayout.EditorGUILayoutInfo m_GuiInfo0 = null;
		public Helpers.EditorGUILayout.EditorGUILayoutInfo m_GuiInfo1 = null;
		public Helpers.EditorGUILayout.EditorGUILayoutInfo m_GuiInfo2 = null;
		public Helpers.EditorGUILayout.EditorGUILayoutInfo m_GuiInfo3 = null;
		public Helpers.EditorGUILayout.EditorGUILayoutInfo m_GuiInfo4 = null;
		public Helpers.EditorGUILayout.EditorGUILayoutInfo m_GuiInfo5 = null;
        public int m_PadRightTimeGameobject = 20;
        public int m_SelectButtonGap        = 10;
    }

	/// <summary>
	/// E debug type base font.
	/// </summary>
	public class EDebugTypeBaseFont
    {
        public FontData m_Message   = new FontData(Color.red);
        public FontData m_CallStack = new FontData(Color.white);
    }
	
	/// <summary>
	/// Font data.
	/// </summary>
    public class FontData
    {
        public int m_FontIndex = 0;
        public int m_FontSize  = 10;
        public Font m_Font     = null;
        public FontStyle m_FontStyle = FontStyle.Normal;
        public Color m_Colour  = Color.black;
        public FontData() {}
        public FontData(Color lColor) { m_Colour = lColor;}
    }

    //--------------------------------------------------
    // these are what apear in the console its self
    //--------------------------------------------------



	/// <summary>
	/// E tab style.
	/// </summary>
    public enum ETabStyle
    {
        HighLight,
        Normal,
        NewItem,
        NoNewItem,
    }
	
	/// <summary>
	/// E display time.
	/// </summary>
    public enum EDisplayTime
    {
        NoTime,
        FromStart,
        RealTime,
        RealTimeShort,
    }

	/// <summary>
	/// E display game object name.
	/// </summary>
	public enum EDisplayGameObjectName
	{
		NoName,
		Name,
		FullName,
	}
    //-------------------------------------------------------------------
    // when in #if !UNITY_EDITOR mode then it will log only these enums
    //-------------------------------------------------------------------
	

	/// <summary>
	/// Data.
	/// </summary>
    public class Data
    {
		public int            m_Count              = 1;
        public int            m_Index              = 0;
        public float          m_TimeFromStart      = 0;
        public DateTime       m_DateTime;
        public string         m_Message            = "";
        public string         m_CallStack          = "";
		public EDebugTypeBase m_EDebugType         = EDebugTypeBase.All;
		public string         m_GameObjectNameFull = "";
        public GameObject     m_GameObject         = null;
		public Data(int index, float time, DateTime dataTime, string message, string callStack, EDebugTypeBase debugType,GameObject gameObject)
        {
            m_Index         = index;
            m_TimeFromStart = time;
            m_DateTime      = dataTime;
            m_Message      = message;
            m_CallStack    = callStack;
            m_EDebugType    = debugType;
            m_GameObject    = gameObject;
			m_Count 		= 0;
            m_GameObjectNameFull = "NULL";
            if(object.ReferenceEquals(m_GameObject,null) == false)
            {
                m_GameObjectNameFull = gameObject.GetGameObjectPath();
            }  
        }


		public  bool Equals( Data other )
		{
            if (object.ReferenceEquals(this.m_GameObject, other.m_GameObject) == false)
            {
                return false;
            }

            if (this.m_Message != other.m_Message)
            {
                return false;
            }

            if (this.m_CallStack != other.m_CallStack)
            {
                return false;
            }
			return true;            
		}

    }
    #endregion


	internal void AddToMessages(Logging consoleInstance, string message, EDebugType debugType, GameObject gameObjectRef)
	{
        consoleInstance.AddToMessages(message, (EDebugTypeBase)debugType, gameObjectRef);
	}


	internal void AddToMessages(string message, EDebugTypeBase debugType, GameObject gameObjectRef)
    {        
        // -------------------------------------------------------------
        //              Clean up Call stack
        //--------------------------------------------------------------
        string stackString        = System.Environment.StackTrace;
        string[] stackStringArray = stackString.Split(m_SetupData.m_LineSplitter);
        string newStackString     = "";

        string loggingFind = "Logging.cs";
        int startIndex = 0;
        // -------------------------------------------------------------
        //              Only Valid call stack parts
        //--------------------------------------------------------------
        for (int i = 0; i < stackStringArray.Length; i++)
        {
            if(true == stackStringArray[i].Contains(loggingFind))
            {
                startIndex = i + 1; // will check all , incase it finds more than 1 of them , depending on my code 
            }
        }

        for (int i = startIndex; i < stackStringArray.Length; i++)
        {
            newStackString += stackStringArray[i] + m_SetupData.m_LineSplitter;
        }

        float time = -1;
        if (false == object.ReferenceEquals(gameObjectRef, null))
        {
            time = Time.realtimeSinceStartup;
        }

        Data newData    = new Data(m_SetupData.m_CurrentIndex, time, DateTime.Now, message, newStackString, debugType, gameObjectRef);
        m_DictionaryListData.AddToList((EDebugTypeBase)debugType, newData);
		AddDictionaryListCountData((EDebugTypeBase)debugType, newData);
        

        m_DictionaryListData[EDebugTypeBase.All].Add(newData);
        AddDictionaryListCountData(EDebugTypeBase.All, newData);

        m_SetupData.m_CurrentIndex++;

    }

    /// <summary>
    /// 
    /// </summary>
    private void RemakeTabAll()
    {
        List<Data> dataList = new List<Data>();

        foreach (var lItem in m_SetupData.m_EDebugTypeDictionary)
        {
            if (lItem.Value == true)
            {
                List<Data> data = m_DictionaryListData.GetList(lItem.Key);
                for (int i = 0; i < data.Count; i++)
                {
                    dataList.Add(data[i]);

                }
            }
        }

        dataList = dataList.OrderBy(o => o.m_Index).ToList();
        m_DictionaryListData[EDebugTypeBase.All] = dataList;


        //-------------------------------------------------------------------
        m_DictionaryListCountData[EDebugTypeBase.All].Clear();
        for (int i = 0; i < dataList.Count; i++)
        {
            AddDictionaryListCountData(EDebugTypeBase.All, dataList[i]);
        }
    }



    #region Display Console

    [MenuItem("Window/Custom/Console Extra")]
    public static void Initialise()
    {
        if (null == m_Window)
        {
            m_Window = (Logging)EditorWindow.GetWindow(typeof(Logging), false, "Console Extra");
        }
        if (null == m_ConsoleInstance)
        {
            m_ConsoleInstance             = new Logging();
            m_ConsoleInstance.m_SetupData = new SetupData();

            m_ConsoleInstance.CreateListOfFonts();
            m_ConsoleInstance.InitialiseEDebugTypeBaseFonts();
            m_ConsoleInstance.InitialiseTabTyle();
            m_ConsoleInstance.Load();
            m_ConsoleInstance.InitialiseGuiFormat();
        }
    }


    /// <summary>
    /// 
    /// </summary>   
    static Logging()
    {
        //Selection.selectionChanged += OnSelectionChange;
        m_EDebugTypeBase         = EDebugTypeBase.All;
        
        m_LoadVariablesOnStart   = false;
        m_Repaint                = true;
        ClearList();
    }


    /// <summary>
    /// 
    /// </summary>
    static void ClearList()
    {
        foreach (EDebugTypeBase lEnumItem in Enum.GetValues(typeof(EDebugTypeBase)))
        {
            //--------------------------------------------------
            //      Safety Check
            //--------------------------------------------------
            if (m_DictionaryListLastViewedCount.ContainsKey(lEnumItem) == false)
            {
                m_DictionaryListLastViewedCount[lEnumItem] = 0;
            }
            m_DictionaryListData.ClearList(lEnumItem);
			m_DictionaryListCountData.ClearList(lEnumItem);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void Save()
    {
        PlayerPrefsX.SetColor(m_LoggingString + "StoreColour",      m_SetupData.m_StoreColour);
        PlayerPrefsX.SetBool (m_LoggingString + "SelectEmptyItem",  m_SetupData.m_SelectEmptyItem);
        PlayerPrefs.SetInt(m_LoggingString    + "DisplayTime",   (int) m_SetupData.m_DisplayTime);
        Save_CscFile();
        PlayerPrefs.SetString(m_LoggingString + "CSC_Directory", m_SetupData.m_CSC_Directory);

        PlayerPrefs.SetInt(m_LoggingString + "TabFont.FontIndex",      m_SetupData.m_TabFont.m_FontIndex);
        PlayerPrefs.SetInt(m_LoggingString + "TabFont.FontSize",       m_SetupData.m_TabFont.m_FontSize);
        PlayerPrefs.SetInt(m_LoggingString + "TabFont.FontStyle", (int)m_SetupData.m_TabFont.m_FontStyle);

        foreach (EDebugType lEnumItemTemp in Enum.GetValues(typeof(EDebugType)))
        {
			EDebugTypeBase enumItem = (EDebugTypeBase)lEnumItemTemp;
            //--------------------------------------------------
            //      Safety Check
            //--------------------------------------------------
			if (m_SetupData.m_EDebugTypeBaseFontDictionary.ContainsKey(enumItem) == false)
            {
				m_SetupData.m_EDebugTypeBaseFontDictionary.Add(enumItem, new EDebugTypeBaseFont());
            }
			FontData fontData = m_SetupData.m_EDebugTypeBaseFontDictionary[enumItem].m_Message;
            PlayerPrefs.SetInt   (m_LoggingString + enumItem + "_Message.FontSize",  fontData.m_FontSize);
            PlayerPrefsX.SetColor(m_LoggingString + enumItem + "_Message.Colour",    fontData.m_Colour);
            PlayerPrefs.SetInt   (m_LoggingString + enumItem + "_Message.FontIndex", fontData.m_FontIndex);

			FontData lCallStack = m_SetupData.m_EDebugTypeBaseFontDictionary[enumItem].m_CallStack;
            PlayerPrefs.SetInt   (m_LoggingString + enumItem + "_CallStack.FontSize",    lCallStack.m_FontSize);
            PlayerPrefsX.SetColor(m_LoggingString + enumItem + "_CallStack.Colour",      lCallStack.m_Colour);
            PlayerPrefs.SetInt   (m_LoggingString + enumItem + "_CallStack.FontIndex",   lCallStack.m_FontIndex);
        }

        foreach (ETabStyle enumItem in Enum.GetValues(typeof(ETabStyle)))
        {
            //--------------------------------------------------
            //      Safety Check
            //--------------------------------------------------
            if (m_SetupData.m_TabStyleDictionary.ContainsKey(enumItem) == false)
            {
                m_SetupData.m_TabStyleDictionary.Add(enumItem, new FontData(Color.red));
            }
            FontData fontData = m_SetupData.m_TabStyleDictionary[enumItem];
            PlayerPrefsX.SetColor(m_LoggingString + enumItem + "_TabStyle.Colour", fontData.m_Colour);
            PlayerPrefs.SetInt   (m_LoggingString + enumItem + "_TabStyle.FontStyle", (int)fontData.m_FontStyle);
        }

		
        foreach (var itemList in m_SetupData.m_EDebugTypeDictionary)
		{
            PlayerPrefsX.SetBool(m_LoggingString + "EDebugTypeAllList_" + itemList.Key, itemList.Value);
		}

        SaveEnumAllFlag();
    }

	/// <summary>
	/// Saves the enum all flag.
	/// </summary>
    public void SaveEnumAllFlag()
    {
        PlayerPrefs.SetInt(m_LoggingString + "_EnumAllFlag", m_SetupData.m_EnumAllFlag);
    }
	

    public void Save_CscFile()
    {
        PlayerPrefs.SetString(m_LoggingString + "CSC_Directory", m_SetupData.m_CSC_Directory);
    }
    /// <summary>
    /// 
    /// </summary>
	public void Load()
    {
        m_SetupData.m_StoreColour           = PlayerPrefsX.GetColor(m_LoggingString + "StoreColour", m_SetupData.m_StoreColour);
        m_SetupData.m_SelectEmptyItem      = PlayerPrefsX.GetBool(m_LoggingString + "SelectEmptyItem", m_SetupData.m_SelectEmptyItem);
        m_SetupData.m_DisplayTime           = (EDisplayTime)PlayerPrefs.GetInt(m_LoggingString + "DisplayTime", (int)m_SetupData.m_DisplayTime);
        m_SetupData.m_CSC_Directory        = PlayerPrefs.GetString(m_LoggingString + "CSC_Directory", m_SetupData.m_CSC_Directory);

        m_SetupData.m_TabFont.m_FontIndex = PlayerPrefs.GetInt(m_LoggingString + "TabFont.FontIndex", m_SetupData.m_TabFont.m_FontIndex);
        m_SetupData.m_TabFont.m_FontSize  = PlayerPrefs.GetInt(m_LoggingString + "TabFont.FontSize", m_SetupData.m_TabFont.m_FontSize);
        m_SetupData.m_TabFont.m_FontStyle = (FontStyle)PlayerPrefs.GetInt(m_LoggingString + "TabFont.FontStyle", (int)m_SetupData.m_TabFont.m_FontStyle);

        //--------------------------------------------------
        //      Safety Check
        //--------------------------------------------------
        if (m_SetupData.m_TabFont.m_FontIndex < m_SetupData.m_FontArray.Length)
        {
            m_SetupData.m_TabFont.m_Font = m_SetupData.m_FontArray[m_SetupData.m_TabFont.m_FontIndex];
        }


		foreach (EDebugType lEnumItemTemp in Enum.GetValues(typeof(EDebugType)))
        {
			EDebugTypeBase lEnumItem = (EDebugTypeBase)lEnumItemTemp;
            //--------------------------------------------------
            //      Safety Check
            //--------------------------------------------------
            if (m_SetupData.m_EDebugTypeBaseFontDictionary.ContainsKey(lEnumItem) == false)
            {
                m_SetupData.m_EDebugTypeBaseFontDictionary.Add(lEnumItem, new EDebugTypeBaseFont());
            }
            FontData message  = m_SetupData.m_EDebugTypeBaseFontDictionary[lEnumItem].m_Message;
            message.m_FontSize  = PlayerPrefs.GetInt   (m_LoggingString + lEnumItem + "_Message.FontSize",  message.m_FontSize);
            message.m_Colour    = PlayerPrefsX.GetColor (m_LoggingString + lEnumItem + "_Message.Colour",    message.m_Colour);
            message.m_FontIndex = PlayerPrefs.GetInt   (m_LoggingString + lEnumItem + "_Message.FontIndex", message.m_FontIndex);
            message.m_FontStyle = (FontStyle)PlayerPrefs.GetInt(m_LoggingString + lEnumItem + "_Message.FontStyle", (int)message.m_FontStyle);

            //--------------------------------------------------
            //      Safety Check
            //--------------------------------------------------
            if (message.m_FontIndex < m_SetupData.m_FontArray.Length)
            {
                message.m_Font = m_SetupData.m_FontArray[message.m_FontIndex];
            }

            

            FontData fontData  = m_SetupData.m_EDebugTypeBaseFontDictionary[lEnumItem].m_CallStack;
            fontData.m_FontSize  = PlayerPrefs.GetInt   (m_LoggingString + lEnumItem + "_CallStack.FontSize", fontData.m_FontSize);
            fontData.m_Colour    = PlayerPrefsX.GetColor (m_LoggingString + lEnumItem + "_CallStack.Colour", fontData.m_Colour);
            fontData.m_FontIndex = PlayerPrefs.GetInt   (m_LoggingString + lEnumItem + "_CallStack.FontIndex", fontData.m_FontIndex);
            fontData.m_FontStyle =(FontStyle)PlayerPrefs.GetInt(m_LoggingString + lEnumItem + "_CallStack.FontStyle", (int)fontData.m_FontStyle);

            //--------------------------------------------------
            //      Safety Check
            //--------------------------------------------------
            if (fontData.m_FontIndex < m_SetupData.m_FontArray.Length)
            {
                fontData.m_Font = m_SetupData.m_FontArray[fontData.m_FontIndex];
            }

            m_SetupData.m_EDebugTypeDictionary.Clear();
            foreach(var item in  (EDebugTypeBase[])Enum.GetValues(typeof(EDebugTypeBase)))
            {
                bool lDefualt = false;               
                m_SetupData.m_EDebugTypeDictionary.Add(item, false);
                m_SetupData.m_EDebugTypeDictionary[item] = PlayerPrefsX.GetBool(m_LoggingString + "EDebugTypeAllList_" + item, lDefualt);
            }

			m_SetupData.m_EnumAllFlag  = PlayerPrefs.GetInt   (m_LoggingString + "_EnumAllFlag", m_SetupData.m_EnumAllFlag);       
        }


        foreach (ETabStyle enumItem in Enum.GetValues(typeof(ETabStyle)))
        {
            //--------------------------------------------------
            //      Safety Check
            //--------------------------------------------------
            if (m_SetupData.m_TabStyleDictionary.ContainsKey(enumItem) == false)
            {
                m_SetupData.m_TabStyleDictionary.Add(enumItem, new FontData(Color.red));
            }
            FontData lFontData  = m_SetupData.m_TabStyleDictionary[enumItem];
            lFontData.m_Colour    = PlayerPrefsX.GetColor(m_LoggingString + enumItem + "_TabStyle.Colour", lFontData.m_Colour);
            lFontData.m_FontStyle = (FontStyle)PlayerPrefs.GetInt(m_LoggingString + enumItem + "_TabStyle.FontStyle", (int)lFontData.m_FontStyle);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <summary>
    /// 
    /// </summary>
    private void OnGUI()
    {
        if (m_SetupData != null)
        {
            m_EDebugTypeBase = Tabs(m_EDebugTypeBase);
            FillInWindow(m_EDebugTypeBase);
            DisplayMessageCallStack();
            
        }
		
        if (m_Repaint == true || IsValidSearchText())
        {
            Repaint();
            m_Repaint = false;
        }
		
		if(m_SetupData != null &&  m_SetupData.m_AudioTracking == true)
		{
			AudioTracking();
		}
    }
	
	/// <summary>
	/// Audios the tracking.
	/// </summary>
	void AudioTracking()
	{
		AudioSource[] sources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
		for (int i = 0; i < sources.Length; i++)
		{
			if (sources[i].clip != null && sources[i].isPlaying)
			{
				AddToMessages(" Clip playing: " + sources[i].clip.name, EDebugTypeBase.Audio , sources[i].gameObject);
			}
		}
	}

	/// <summary>
	/// Raises the inspector update event.
	/// </summary>
    void OnInspectorUpdate()
    {

        if(m_LoadVariablesOnStart == false)
        {
            m_SetupData = new SetupData();
            CreateListOfFonts();
            InitialiseEDebugTypeBaseFonts();
            InitialiseTabTyle();
            Load();
			InitialiseGuiFormat();
            m_LoadVariablesOnStart = true;
            
        }
        if (m_Repaint == true || IsValidSearchText())
        {
            Repaint();
            m_Repaint = false;
        }
    }
	

    /// <summary>
    /// Initialises the GUI format.
    /// </summary>
    internal void InitialiseGuiFormat()
	{
		m_SetupData.m_GuiInfo0 = new Helpers.EditorGUILayout.EditorGUILayoutInfo( 0, m_SetupData.m_SpaceAtBeginning * 0, 0, 10, m_SetupData.m_InformationHeight);
        m_SetupData.m_GuiInfo1 = new Helpers.EditorGUILayout.EditorGUILayoutInfo( 1, m_SetupData.m_SpaceAtBeginning * 0, m_SetupData.m_NameWidth, m_SetupData.m_InformationWidth, m_SetupData.m_InformationHeight);
		m_SetupData.m_GuiInfo2 = new Helpers.EditorGUILayout.EditorGUILayoutInfo( 1, m_SetupData.m_SpaceAtBeginning * 1, m_SetupData.m_NameWidth, m_SetupData.m_InformationWidth, m_SetupData.m_InformationHeight);
        m_SetupData.m_GuiInfo3 = new Helpers.EditorGUILayout.EditorGUILayoutInfo( 1, m_SetupData.m_SpaceAtBeginning * 2, m_SetupData.m_NameWidth, m_SetupData.m_InformationWidth, m_SetupData.m_InformationHeight);
        m_SetupData.m_GuiInfo4 = new Helpers.EditorGUILayout.EditorGUILayoutInfo( 1, m_SetupData.m_SpaceAtBeginning * 3, m_SetupData.m_NameWidth, m_SetupData.m_InformationWidth, m_SetupData.m_InformationHeight);
        m_SetupData.m_GuiInfo5 = new Helpers.EditorGUILayout.EditorGUILayoutInfo( 1, m_SetupData.m_SpaceAtBeginning * 4, m_SetupData.m_NameWidth, m_SetupData.m_InformationWidth, m_SetupData.m_InformationHeight);
    }

    /// <summary>
    /// 
    /// </summary>
    internal void InitialiseTabTyle()
    {
        foreach (ETabStyle lEnumItem in Enum.GetValues(typeof(ETabStyle)))
        {
            //--------------------------------------------------
            //      Safety Check
            //--------------------------------------------------
            if (m_SetupData.m_TabStyleDictionary.ContainsKey(lEnumItem) == false)
            {
                m_SetupData.m_TabStyleDictionary.Add(lEnumItem, new FontData());
            }
        }

        m_SetupData.m_TabStyleDictionary[ETabStyle.HighLight].m_Colour = new Color(0.3f, 0.3f, 0.3f);
        m_SetupData.m_TabStyleDictionary[ETabStyle.Normal].m_Colour    = new Color(0.5f, 0.5f, 0.5f);
        m_SetupData.m_TabStyleDictionary[ETabStyle.NewItem].m_Colour   = Helpers.Colour.Colour256ToColour(210, 36, 46);
		m_SetupData.m_TabStyleDictionary[ETabStyle.NoNewItem].m_Colour = Helpers.Colour.Colour256ToColour(11, 196, 89); // nice green
    }


    /// <summary>
    /// 
    /// </summary>
    internal void InitialiseEDebugTypeBaseFonts()
    {
		foreach (EDebugType enumItemTemp in Enum.GetValues(typeof(EDebugType)))
        {
			EDebugTypeBase enumItem = (EDebugTypeBase)enumItemTemp;
            //--------------------------------------------------
            //      Safety Check
            //--------------------------------------------------
            if (m_SetupData.m_EDebugTypeBaseFontDictionary.ContainsKey(enumItem) == false)
            {
                m_SetupData.m_EDebugTypeBaseFontDictionary.Add(enumItem, new EDebugTypeBaseFont());
            }
        }
    }


	/// <summary>
	/// Determines whether this instance is valid search text.
	/// </summary>
    bool IsValidSearchText()
    {
        if (m_SearchMenu != null && m_SearchMenu.m_SetupData.m_SearchText != "")
        {
            return true;
        }
        return false;
    }

	/// <summary>
	/// Determines whether this instance is valid search text the specified lString.
	/// </summary>
    bool IsValidSearchText(ref string lString)
    {
        if(m_SearchMenu != null && m_SearchMenu.m_SetupData.m_SearchText != "")
        {
            lString = m_SearchMenu.m_SetupData.m_SearchText;
            return true;
        }
        return false;
    }
	
    /// <summary>
    /// 
    /// </summary>
    private void FillInWindow(EDebugTypeBase debugType)
    {
        List<Data> dataList = null;

        if (m_SetupData.m_Compressed == true)
        {
            dataList = m_DictionaryListCountData.GetList(debugType);
        }
        else
        {
            dataList = m_DictionaryListData.GetList(debugType);
        }




        DisplayClearButton(debugType, ref dataList);
        // reset BackGround
        GUI.backgroundColor  = Color.white;
		m_SetupData.m_ScrollPos = EditorGUILayout.BeginScrollView(m_SetupData.m_ScrollPos, true, true);

		Helpers.EditorGUILayout.Space(3);

		DisplayAllEnumButton(debugType);

        
        for (int i = 0; i < dataList.Count; i++)
        {
			Data lData = dataList[i];
			if(ShouldDisplayData(lData) == true)
			{
            	//EditorGUILayout.BeginHorizontal();
				DisplayMessageString(lData);
            	DisplayMessageGameObjectName(lData);
                

               // EditorGUILayout.EndHorizontal();
				
            	
			}

        }


        EditorGUILayout.EndScrollView();
    }


	/// <summary>
	/// Displaies the data.
	/// </summary>
	private bool ShouldDisplayData(Data data)
	{
		string searchString = "";            
		bool validSearch    = IsValidSearchText(ref searchString);          
		bool found          = true;
		
        if(validSearch == true)
		{
			found = false;
			switch(m_SearchMenu.m_SetupData.SearchType)
			{
			case SearchMenu.ESearchType.Message:
                    found = data.m_Message.Contains(searchString);
				break;
				
			case SearchMenu.ESearchType.CallStack:
                    found = data.m_CallStack.CaseInsensitiveContains(searchString);
				break;
				
			case SearchMenu.ESearchType.GameObjectName:
                    found = data.m_GameObjectNameFull.CaseInsensitiveContains(searchString);
				break;

                case SearchMenu.ESearchType.All:
                    found = data.m_Message.CaseInsensitiveContains(searchString);
                    if (found == false)
                    {
                        found = data.m_CallStack.CaseInsensitiveContains(searchString);
                    }
                    if (found == false)
                    {
                        found = data.m_GameObjectNameFull.CaseInsensitiveContains(searchString);
                    }
                    break;
            }
        }
		return found;
    }

    /// <summary>
    /// Displaies all enum button.
	/// </summary>
	private void DisplayAllEnumButton(EDebugTypeBase debugType)
	{
        if (debugType == EDebugTypeBase.All)
		{
			const int spaceAtBeginning = 20;
			const int nameWidth        = 150;
			const int informationWidth = 150;
            const int nameHieght       = 20;
            Helpers.EditorGUILayout.EditorGUILayoutInfo lInfo1 = new Helpers.EditorGUILayout.EditorGUILayoutInfo(2, spaceAtBeginning * 1, nameWidth, informationWidth, nameHieght);

            Dictionary <EDebugTypeBase, bool> EDebugTypeAllListTemp = new Dictionary<EDebugTypeBase, bool>(m_SetupData.m_EDebugTypeDictionary);

            m_SetupData.m_EDebugTypeDictionary = Helpers.EditorGUILayout.EnumDictionaryExcept<EDebugTypeBase, EDebugSpecialList>("EnumList", ref m_SetupData.m_EnumAllFlag, lInfo1, true);

            if (DictionaryComparer<EDebugTypeBase, bool>(EDebugTypeAllListTemp, m_SetupData.m_EDebugTypeDictionary) == false)
			{
				RemakeTabAll();
				SaveEnumAllFlag();				
			}

            EditorGUILayout.BeginHorizontal();
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
            EditorGUILayout.EndHorizontal();
            Helpers.EditorGUILayout.Space(1);			
		}
	}

	/// <summary>
	/// Displaies the message string.
	/// </summary>
	private void DisplayMessageString(Data data)
	{
        EditorGUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.white;
        if (m_SetupData.m_SelectedItem == data)
        {
            GUI.backgroundColor = Color.green;
        }
        if (GUILayout.Button("", GUILayout.Width(10), GUILayout.Height(10)) == true)
        {
            if (data.m_GameObject != null)
            {                
                Selection.activeGameObject    = data.m_GameObject;
                EditorGUIUtility.PingObject(Selection.activeGameObject);
            }
            m_SetupData.m_SelectedItem = data;
        }
        GUI.backgroundColor = Color.white;
        //--------------------------------------------------
        //      Safety Check
        //--------------------------------------------------
        if (false == m_SetupData.m_EDebugTypeBaseFontDictionary.ContainsKey(data.m_EDebugType))
		{
            m_SetupData.m_EDebugTypeBaseFontDictionary.Add((EDebugTypeBase)data.m_EDebugType, new EDebugTypeBaseFont());
		}
		
		GUIStyle myButtonStyle         = new GUIStyle();

		myButtonStyle.fontSize         = m_SetupData.m_EDebugTypeBaseFontDictionary[data.m_EDebugType].m_Message.m_FontSize;
		myButtonStyle.font             = m_SetupData.m_EDebugTypeBaseFontDictionary[data.m_EDebugType].m_Message.m_Font;
		myButtonStyle.fontStyle        = m_SetupData.m_EDebugTypeBaseFontDictionary[data.m_EDebugType].m_Message.m_FontStyle;
        myButtonStyle.normal.textColor = m_SetupData.m_EDebugTypeBaseFontDictionary[data.m_EDebugType].m_Message.m_Colour;
        string timeString = "";
		switch(m_SetupData.m_DisplayTime)
		{
		case EDisplayTime.NoTime:
			timeString = "";
			break;
			
		case EDisplayTime.FromStart:
			timeString = "Time " + data.m_TimeFromStart.ToString();
			break;
			
		case EDisplayTime.RealTime:
			timeString = "Time " + String.Format("{0:HH:mm:ss.ffffff}", data.m_DateTime);
			break;
			
		case EDisplayTime.RealTimeShort:
			timeString = "Time " + String.Format("{0:mm:ss.fff}", data.m_DateTime);
			break;
		}
        EditorGUILayout.LabelField("", GUILayout.Width(m_SetupData.m_SelectButtonGap));
        DisplayCompressed(data, m_SetupData.m_Compressed);
        EditorGUILayout.TextArea(timeString.PadRight(m_SetupData.m_PadRightTimeGameobject) + "  " + data.m_Message, myButtonStyle, GUILayout.Width(1000));
        
        EditorGUILayout.EndHorizontal();
	}

	/// <summary>
	/// Displaies the name of the message game object.
	/// </summary>
	void DisplayMessageGameObjectName(Data lData)
	{
        string nameString = "GameObject -->>   ";
		
		GUIStyle myButtonObjectNameStyle         = new GUIStyle();
		myButtonObjectNameStyle.normal.textColor = m_SetupData.m_EDebugTypeBaseFontDictionary[lData.m_EDebugType].m_Message.m_Colour;
		myButtonObjectNameStyle.fontSize         = m_SetupData.m_EDebugTypeBaseFontDictionary[lData.m_EDebugType].m_Message.m_FontSize;
		myButtonObjectNameStyle.font             = m_SetupData.m_EDebugTypeBaseFontDictionary[lData.m_EDebugType].m_Message.m_Font;
		myButtonObjectNameStyle.fontStyle        = m_SetupData.m_EDebugTypeBaseFontDictionary[lData.m_EDebugType].m_Message.m_FontStyle;

        EditorGUILayout.BeginHorizontal();

        if (m_SetupData.m_Compressed == true)
        {
            Helpers.EditorGUILayout.LabelFieldBlank(38);
        }
        Helpers.EditorGUILayout.LabelFieldBlank(10 + m_SetupData.m_SelectButtonGap);
        EditorGUILayout.TextArea(nameString.PadRight(m_SetupData.m_PadRightTimeGameobject) + lData.m_GameObjectNameFull, myButtonObjectNameStyle, GUILayout.Width(300));
        EditorGUILayout.EndHorizontal();
	}
	
	/// <summary>
	/// Displaies the compressed.
	/// </summary>
	void DisplayCompressed(Data data, bool compressed)
	{
		if(true == compressed)
		{
			GUIStyle lMyButtonObjectNameStyle         = new GUIStyle();
			lMyButtonObjectNameStyle.normal.textColor = m_SetupData.m_EDebugTypeBaseFontDictionary[data.m_EDebugType].m_Message.m_Colour;
			lMyButtonObjectNameStyle.fontSize         = m_SetupData.m_EDebugTypeBaseFontDictionary[data.m_EDebugType].m_Message.m_FontSize;
			lMyButtonObjectNameStyle.font             = m_SetupData.m_EDebugTypeBaseFontDictionary[data.m_EDebugType].m_Message.m_Font;
			lMyButtonObjectNameStyle.fontStyle        = m_SetupData.m_EDebugTypeBaseFontDictionary[data.m_EDebugType].m_Message.m_FontStyle;
            string number = "(" + data.m_Count.ToString() + ")";
            EditorGUILayout.TextArea(number.PadRight(10), lMyButtonObjectNameStyle, GUILayout.Width(40));
		}
	}

	/// <summary>
	/// Displaies the message call stack.
	/// </summary>
	void DisplayMessageCallStack()
	{
        EditorGUILayout.Space();
        m_SetupData.m_ScrollPosCallStack = EditorGUILayout.BeginScrollView(m_SetupData.m_ScrollPosCallStack, true, true, GUILayout.Height(70));

        if (null != m_SetupData.m_SelectedItem)
        {
            //------------------------------------------------------------------------------
            //        if its checked, the display all the stack trace
            //------------------------------------------------------------------------------

            GUIStyle callStackButtonStyle = new GUIStyle();

            callStackButtonStyle.normal.textColor = m_SetupData.m_EDebugTypeBaseFontDictionary[m_SetupData.m_SelectedItem.m_EDebugType].m_CallStack.m_Colour;
            callStackButtonStyle.fontSize         = m_SetupData.m_EDebugTypeBaseFontDictionary[m_SetupData.m_SelectedItem.m_EDebugType].m_CallStack.m_FontSize;
            callStackButtonStyle.font             = m_SetupData.m_EDebugTypeBaseFontDictionary[m_SetupData.m_SelectedItem.m_EDebugType].m_CallStack.m_Font;
            callStackButtonStyle.fontStyle        = m_SetupData.m_EDebugTypeBaseFontDictionary[m_SetupData.m_SelectedItem.m_EDebugType].m_CallStack.m_FontStyle;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(m_SetupData.m_ButtonStartWidth / 2));
            EditorGUILayout.TextArea(m_SetupData.m_SelectedItem.m_CallStack, callStackButtonStyle);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
        }
        EditorGUILayout.EndScrollView();

    }

	/// <summary>
	/// Displaies the clear button.
	/// </summary>
	void DisplayClearButton(EDebugTypeBase debugType, ref List<Data> dataList)
	{
		//------------------------------------------------------------------------------
		//                      If there is information then present the button
		//------------------------------------------------------------------------------
		if (dataList.Count > 0)
		{
			Helpers.EditorGUILayout.Space(4);
			EditorGUILayout.BeginHorizontal();
			int lSpace = Helpers.EditorGUILayout.ButtonSpaceCentre(m_SetupData.m_ButtonWidth, (int)position.width, true);
			
			Helpers.EditorGUILayout.LabelFieldBlank(lSpace);
			
			if (GUILayout.Button(m_SetupData.m_ClearButtonText, GUILayout.Width(m_SetupData.m_ButtonWidth), GUILayout.Height(m_SetupData.m_ButtonHeight)) == true)
			{
                //------------------------------------------------------------------------------
                //        Clear the data
                //------------------------------------------------------------------------------

                if (debugType == EDebugTypeBase.All)
                {
                    m_DictionaryListData.ClearAll();

                    List<EDebugTypeBase> keyList = new List<EDebugTypeBase>(m_DictionaryListLastViewedCount.Keys);
                    foreach (EDebugTypeBase itemKey in keyList)
                    {
                        m_DictionaryListLastViewedCount[itemKey] = 0;
                    }
                }
                else
                {
                    m_DictionaryListData.ClearList(debugType);
                    m_DictionaryListLastViewedCount[debugType] = 0;
                }


				RemakeTabAll();
			}
			
			EditorGUILayout.EndHorizontal();
            Helpers.EditorGUILayout.Space(2);
        }
	}


    /// <summary>
    /// 
    /// <returns></returns>
    private EDebugTypeBase Tabs(EDebugTypeBase selectedItem)
    {
        string tick  = ((char)0x221A).ToString();
        string xross = "x";
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("", GUILayout.Width(position.width - (m_SetupData.m_ButtonWidth * 2) - 10 - 30 - 25 - 25 - 10));
        string audioTrackingString = "A";
        if(m_SetupData.m_AudioTracking == true)
        {

            audioTrackingString += tick;
        }
        else
        {
            audioTrackingString += xross;
        }

        if (GUILayout.Button(audioTrackingString, GUILayout.Width(30), GUILayout.Height(20)))
		{
			m_SetupData.m_AudioTracking = EditorUtility.DisplayDialog("Audio Trackinge?",
			                                                     "Do you wish to Enable Audio Tracking ", 
			                                                     "Enable", 
			                                                     "Disable");
			Repaint();
		}


	
		if (GUILayout.Button("C", GUILayout.Width(20), GUILayout.Height(20)))
		{
			m_SetupData.m_Compressed = !m_SetupData.m_Compressed;
			Repaint();
		}



		if (GUILayout.Button("Search Menu!", GUILayout.Width(m_SetupData.m_ButtonWidth), GUILayout.Height(20)))
		{
			Repaint();
            if (m_SearchMenu == null)
            {
                m_SearchMenu = new SearchMenu(this);
            }
		}

		if (GUILayout.Button("Settings Menu!", GUILayout.Width(m_SetupData.m_ButtonWidth), GUILayout.Height(20)))
		{
			Repaint();
            if (m_Settings == null)
            {
                m_Settings = new Settings(this);
            }
        }
        GUILayout.EndHorizontal();


		m_SetupData.m_ScrollPosTab = EditorGUILayout.BeginScrollView(m_SetupData.m_ScrollPosTab, true, false, GUILayout.Height(55));
		m_SetupData.m_ScrollPosTab = new Vector2(m_SetupData.m_ScrollPosTab.x, 0);
        GUILayout.Space(m_SetupData.m_StartSpace);

        GUIStyle buttonStyle       = new GUIStyle(GUI.skin.button);
        buttonStyle.padding.bottom = 8;
        buttonStyle.font           = (m_SetupData.m_TabFont.m_Font != null) ? m_SetupData.m_TabFont.m_Font : buttonStyle.font;
        buttonStyle.fontSize       = m_SetupData.m_TabFont.m_FontSize;
        buttonStyle.fontStyle      = m_SetupData.m_TabFont.m_FontStyle;

        
        GUILayout.BeginHorizontal();
        {   
            //Create a row of buttons
            foreach (EDebugTypeBase enumItem in Enum.GetValues(typeof(EDebugTypeBase)))
            {
                ETabStyle tabStyle     = ETabStyle.Normal;
                int lastViewedCount     = m_DictionaryListData.Count(enumItem);
                bool isButtonSelectable = (lastViewedCount > 0);

                //--------------------------------------------------
                //  sets back ground colour
                //--------------------------------------------------
                if (isButtonSelectable == false)
                {
                    tabStyle = ETabStyle.NoNewItem;
                }

                if (m_DictionaryListLastViewedCount[enumItem] != lastViewedCount)
                {
                    tabStyle = ETabStyle.NewItem;
                }

                if (selectedItem == enumItem)
                {
                    tabStyle = ETabStyle.HighLight;
                }

                if (m_SetupData.m_TabStyleDictionary.ContainsKey(tabStyle) == true)
                {
                    GUI.backgroundColor = m_SetupData.m_TabStyleDictionary[tabStyle].m_Colour;
                    buttonStyle.fontStyle = m_SetupData.m_TabStyleDictionary[tabStyle].m_FontStyle;
                }
                //---------------------------------------------------------------------------------
                //    Create the button name, with a number of there are new items
                //---------------------------------------------------------------------------------
				string lButtonName = (lastViewedCount == 0) ? enumItem.ToString() : enumItem.ToString() + "(" + lastViewedCount.ToString() + ")";

                if (GUILayout.Button(lButtonName, buttonStyle) == true && 
                    (	isButtonSelectable == true  || 
				 		m_SetupData.m_SelectEmptyItem == true //||
				 		//TODO Helpers.Enums.IsEnumInEnumItem<EDebugSpecialList, EDebugTypeBase>(lEnumItem) == true
				 ))
                {
                    m_DictionaryListLastViewedCount[enumItem] = lastViewedCount;
                    
                    //---------------------------------------------------------------------------------
                    //    if click on a the tab AGAIN then it will make the items all apear or disapear
                    //---------------------------------------------------------------------------------
                    if (selectedItem != enumItem)
                    {
                        m_SetupData.m_SelectedItem = null;
                    }
                    selectedItem = enumItem;
                }
            }
        }
        Helpers.EditorGUILayout.LabelFieldBlank(10); // gap at the tend were the scrollbar is
        GUILayout.EndHorizontal();
        EditorGUILayout.EndScrollView();

        return selectedItem;
    }

    private SearchMenu m_SearchMenu;
    private Settings m_Settings;

    public static bool DictionaryComparer<TKey, TValue>(Dictionary<TKey, TValue> x, Dictionary<TKey, TValue> y)
    {
        if (x.Count != y.Count)
            return false;

        foreach (var lX in x)
        {
            bool foundAndSame = false;
            foreach (var lY in y)
            {
                if (lX.Key.Equals(lY.Key))
                {
                    if (lX.Value.Equals(lY.Value))
                    {
                        foundAndSame = true;
                        break;
                    }
                }
            }
            if (foundAndSame == false)
            {
                return false;
            }
        }

        return true;

    }

    /// <summary>
    /// 
    /// </summary>
    internal void CreateListOfFonts()
    {
        //----------------------------------------------
        //          Safety Checks
        //----------------------------------------------
        List<Font> fontList        = new List<Font>();
        m_SetupData.m_FontArray    = Resources.FindObjectsOfTypeAll(typeof(Font)) as Font[];
        m_SetupData.m_FontNameList = new List<string>();
        foreach (Font font in m_SetupData.m_FontArray)
        {
            if (font.fontNames.Length > 0)
            {
                if (m_SetupData.m_FontNameList.Contains(font.fontNames[0]) == false)
                {
                    m_SetupData.m_FontNameList.Add(font.fontNames[0]);
                    fontList.Add(font);
                }
            }
        }       
    }

    #endregion
}
#endif
