using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.IO;

public class Global : MonoBehaviour
{

    #region Data From Files
    /*
        -Gatonedas (int)
        -Gatonedas per click index (int)
        -Gatonedas per click list(List<int>)
    */
    private string generalFile = "data.json";
    private List<int> generalList;
    private List<int> saveMe;
    #endregion

    #region Layers
    [SerializeField]
    private GameObject MenuLayer;
    [SerializeField]
    private GameObject LevelLayer;
    #endregion

    #region Gatonedas Variables
    private const int MAXGATONEDAS = 999999999;
    [SerializeField]
    private int gatonedas;
    [SerializeField]
    private Text GatonedasText;

    private List<int> gatonedasPerClick;
    private int gatonedasPerClickIndex;

    [SerializeField]
    private Text ActualGatonedasPerClick;
    [SerializeField]
    private Text NextGatonedasPerClick;
    #endregion
    #region Gatonedas Functions
    public void AddGatonedas()
    {
        if (gatonedas + gatonedasPerClick[gatonedasPerClickIndex+1] > MAXGATONEDAS)
        {
            gatonedas = MAXGATONEDAS;
            return;
        }

        gatonedas += gatonedasPerClickIndex;
    }
    private void saveGatonedas()
    {
        saveMe.Add(gatonedas);
    }
    private void loadGatonedas()
    {
        gatonedas = generalList[0];
    }
    private void updateGatonedas()
    {
        string st = gatonedas.ToString("D9");
        GatonedasText.text = string.Format("{0}{1}{2}.{3}{4}{5}.{6}{7}{8}",
            st[0], st[1], st[2], st[3], st[4], st[5], st[6], st[7], st[8]);
    }
    #endregion
    #region GatonedasPerClick Functions
    public void AddGatonedasPerClickIndex()
    {
        if (gatonedasPerClickIndex + 1 == gatonedasPerClick.Count)
            return;
        if (gatonedas < gatonedasPerClick[gatonedasPerClickIndex+1])
            return;

        gatonedasPerClickIndex++;
        gatonedas -= gatonedasPerClick[gatonedasPerClickIndex];
    }
    private void loadGatonedasPerClickIndex()
    {
        gatonedasPerClickIndex = generalList[1];
    }
    private void saveGatonedasPerClickIndex()
    {
        saveMe.Add(gatonedasPerClickIndex);
    }

    private void loadGatonedasPerClick()
    {
        gatonedasPerClick = new List<int>();
        for (int i = 2; i < generalList.Count; i++)
            gatonedasPerClick.Add(generalList[i]);
        gatonedasPerClickIndex = 1;
    }
    private void saveGatonedasPerClick()
    {
        for (int i = 0; i < gatonedasPerClick.Count; i++)
            saveMe.Add(gatonedasPerClick[i]);
    }
    private void updateGatonedasPerClick()
    {
        ActualGatonedasPerClick.text = gatonedasPerClickIndex.ToString("D4");

        if (gatonedasPerClickIndex + 1 == gatonedasPerClick.Count)
            NextGatonedasPerClick.text = "MAX.";
        else
        {
            string value = "";
            string letter = "";
            compressInt(ref value, ref letter);
            NextGatonedasPerClick.text = value + letter;
        }
    }
    private void compressInt(ref string value, ref string letter)
    {
        int unit = 1000;
        if (gatonedasPerClick[gatonedasPerClickIndex+1] < unit)
        {
            value = gatonedasPerClick[gatonedasPerClickIndex+1].ToString("D3");
            letter = " ";
            return;
        }
        int exp = (int)(Mathf.Log(gatonedasPerClick[gatonedasPerClickIndex+1]) / Mathf.Log(unit));
        letter = "kMGTPE"[exp - 1].ToString();
        int val = (int)(gatonedasPerClick[gatonedasPerClickIndex+1] / Mathf.Pow(unit, exp));
        value = val.ToString("D3");
    }
    #endregion

    #region Score
    private List<int> score;
    [SerializeField]
    private Text scoreText;
    #endregion
    #region Score Functions
    private void setScore(int newScore)
    {
        if(newScore > score[actualLevel])
            score[actualLevel] = newScore;
    }
    private void updateScore()
    {
        if (scoreText == null)
            return;
        scoreText.text = "Level: " + score[actualLevel].ToString("D2");
    }
    #endregion

    #region Block
    private string blockFile = "block.json";
    [SerializeField]
    private List<bool> blocked;
    private List<int> prize;
    [SerializeField]
    private GameObject blockImage;
    [SerializeField]
    private GameObject unlockMessage;
    #endregion
    #region Block Functions
    public void unlock()
    {
        if(prize[actualLevel] <= gatonedas)
        {
            gatonedas -= prize[actualLevel];
            blocked[actualLevel] = false;
            unlockMessage.SetActive(false);
            blockImage.SetActive(false);
        }
    }
    private void blockMessage()
    {
        unlockMessage.SetActive(true);
        unlockMessage.transform.Find("necesario").GetComponent<Text>().text = prize[actualLevel].ToString("D9");
    }
    private void saveBlocked()
    {
        List<int> list = new List<int>();
        foreach (var item in blocked)
        {
            list.Add((item == false) ? 1 : 0);
        }
        list.Add(-1);
        foreach (var item in prize)
        {
            list.Add(item);
        }
        list.Add(-1);
        foreach (var item in score)
        {
            list.Add(item);
        }
        SaveAndLoad.Write(ref list, blockFile);
    }
    private void loadBlocked()
    {
        List<int> integer = SaveAndLoad.Read(blockFile);
        blocked = new List<bool>();
        prize = new List<int>();
        score = new List<int>();
        int ch = 0;
        foreach (int item in integer)
        {
            if(item == -1)
            {
                ch++;
                continue;
            }
            switch (ch)
            {
                case 0:
                    blocked.Add((item == 0) ? true : false);
                    break;
                case 1:
                    prize.Add(item);
                    break;
                case 2:
                    score.Add(item);
                    break;
                default:
                    break;
            }
        }
    }
    #endregion

    #region Level Variable
    [System.Serializable]
    struct Level
    {
        public List<int> clicksToChangeLevels;
        public bool IsNull()
        {
            return clicksToChangeLevels == null;
        }
    }

    [SerializeField]
    private List<GameObject> pets;
    [SerializeField]
    private List<GameObject> petsInGame;
    [SerializeField]
    private Text petText;

    private new string levelFile = "level.json";
    private List<Level> levelSelected;

    [SerializeField]
    private Text LevelInLevelText;
    private int actualLevel;
    private int actualLevelInLevel;
    private int clicksOnLevel;
    private bool playing;

    [SerializeField]
    private Text TimeText;
    private float startTime;
    private int timeRemainder;

    [SerializeField]
    private Text Percent;
    private int percent;

    #endregion
    #region Level Function
    private void loadLevel()
    {
        List<int> fileData = SaveAndLoad.Read(levelFile);
        levelSelected = new List<Level>();
        int index = 0;

        for (int i = 0; i < fileData.Count; i++)
        {
            if (fileData[i] == -1)
            {
                index++;
                continue;
            }
            if(levelSelected.Count == index)
                levelSelected.Add(new Level());

            Level b = new Level();

            if (levelSelected[index].IsNull())
                b.clicksToChangeLevels = new List<int>();
            else
                b.clicksToChangeLevels = levelSelected[index].clicksToChangeLevels;

            b.clicksToChangeLevels.Add(fileData[i]);
            levelSelected[index] = b;
        }
    }
    private void changeLevel()
    {
        if (levelSelected[actualLevel].clicksToChangeLevels.Count - 1 == actualLevelInLevel)
        {
            EndGame();
            return;
        }
        actualLevelInLevel++;
        startTime = Time.time;
        clicksOnLevel = 0;
        setScore(actualLevelInLevel);
        Save();
    }
    private void updatePercent()
    {
        if (!playing)
            return;

        // Calcula el factor que representa el 
        // porcentaje actual del nivel.
        float factor = (float)clicksOnLevel / levelSelected[actualLevel].clicksToChangeLevels[actualLevelInLevel];
        // Cambia el factor a valor de porciento.
        int per100 = Mathf.FloorToInt(factor * 100f);

        // Asigna el porcentaje a "levelPercent".
        percent = per100;
    }
    private void updateTime()
    {
        if (!playing)
            return;

        timeRemainder = 20 - Mathf.CeilToInt(Time.time - startTime);

        if (timeRemainder == 0)
            changeLevel();
    }
    private void updatePet()
    {
        petText.text = pets[actualLevel].name;
    }
    private void updateLevel()
    {
        if (!playing)
            return;

        milkSize();

        LevelInLevelText.text = "Level: " + actualLevelInLevel.ToString("D2");

        updateTime();
        TimeText.text = timeRemainder.ToString("D2");

        updatePercent();
        if(percent < 100)
            Percent.text = percent.ToString("D2") + "%";

        if (clicksOnLevel >= levelSelected[actualLevel].clicksToChangeLevels[actualLevelInLevel])
            changeLevel();

        if (timeRemainder <= 0)
            EndGame();
    }

    public void ChangeSelect(bool right)
    {
        if(right)
        {
            if(actualLevel + 1 < levelSelected.Count)
            {
                pets[actualLevel].SetActive(false);
                actualLevel++;
                pets[actualLevel].SetActive(true);

                if (blocked[actualLevel] == true)
                    blockImage.SetActive(true);
                else
                    blockImage.SetActive(false);
            }
        }
        else
        {
            if (actualLevel > 0)
            {
                pets[actualLevel].SetActive(false);
                actualLevel--;
                pets[actualLevel].SetActive(true);

                if (blocked[actualLevel] == true)
                    blockImage.SetActive(true);
                else
                    blockImage.SetActive(false);
            }
        }
    }
    public void StartLevel()
    {
        if (blocked[actualLevel])
        {
            blockMessage();
            return;
        }
        MenuLayer.SetActive(false);
        LevelLayer.SetActive(true);
        petsInGame[actualLevel].SetActive(true);

        percent = 0;
        startTime = Time.time;
        playing = true;
    }
    private void EndGame()
    {
        MenuLayer.SetActive(true);
        LevelLayer.SetActive(false);
        petsInGame[actualLevel].SetActive(false);

        setScore(actualLevelInLevel);

        actualLevelInLevel = 0;
        percent = 0;
        startTime = Time.time;
        playing = false;
        Save();
    }
    #endregion

    #region Milk
    private float milkOriginalY;
    [SerializeField]
    public RectTransform milk;

    #endregion
    #region Milk Functions
    private void milkSize()
    {
        // Calcula la talla del objeto
        // usando el porcentaje del nivel
        // y el tamaño original del objeto.
        float sizeY = milkOriginalY / 100;
        sizeY *= percent;
        // Si el tamaño calculado sobrepasa
        // el original, se reasignara el
        // tamaño original.
        sizeY = sizeY < milkOriginalY ? sizeY : milkOriginalY;

        // Se asigna el valor de "sizeY" a la escala
        // del objeto.
        milk.sizeDelta = new Vector2(milk.sizeDelta.x, sizeY);
    }
    #endregion


    #region  Save and Load
    public void Save()
    {
        saveMe = new List<int>();
        saveGatonedas();
        saveGatonedasPerClickIndex();
        saveGatonedasPerClick();

        saveBlocked();

        SaveAndLoad.Write(ref saveMe, generalFile);
    }
    public void Load()
    {
        generalList = SaveAndLoad.Read(generalFile);
        loadGatonedas();
        loadGatonedasPerClickIndex();

        // ULTIMO
        loadGatonedasPerClick();
        loadBlocked();

        loadLevel();
    }
    #endregion


    void Start()
    {
        Init();
        milkOriginalY = milk.rect.height;
        Load();
        blockImage.SetActive(blocked[actualLevel]);
    }

    void Update()
    {
        updatePet();
        updateGatonedas();
        updateGatonedasPerClick();
        updateLevel();
        updateScore();
    }

    public void Click()
    {
        Debug.Log("Gatoneda");
        clicksOnLevel += gatonedasPerClickIndex;
        AddGatonedas();
    }
    public void Pause()
    {
        Time.timeScale = 0;
    }
    public void EndPause()
    {
        Time.timeScale = 1;
    }
    public void Quit()
    {
        Save();
        Application.Quit();
    }



    private void Init()
    {
        // block.json File
        List<int> st1 = new List<int>();
        st1.Add(0);
        st1.Add(0);
        st1.Add(0);
        st1.Add(0);
        st1.Add(-1);
        st1.Add(0);
        st1.Add(5000);
        st1.Add(10000);
        st1.Add(50000);
        st1.Add(-1);
        st1.Add(0);
        st1.Add(0);
        st1.Add(0);
        st1.Add(0);
        SaveAndLoad.Initialize(st1, blockFile);

        // level.json File
        List<int> st2 = new List<int>();

        for(int i = 0; i < 50; i++)
            st2.Add( Mathf.FloorToInt( 50 * Mathf.Log(i+1) + 10 ));
        st2.Add(-1);
        for(int i = 0; i < 50; i++)
            st2.Add(Mathf.FloorToInt(100 * Mathf.Log(i + 1) + 150));
        st2.Add(-1);
        for (int i = 0; i < 50; i++)
            st2.Add(Mathf.FloorToInt(150 * Mathf.Log(i + 1) + 450));
        st2.Add(-1);
        for (int i = 0; i < 50; i++)
            st2.Add(Mathf.FloorToInt(200 * Mathf.Log(i + 1) + 950));
        st2.Add(-1);

        SaveAndLoad.Initialize(st2, levelFile);

        // data.json File
        List<int> st3 = new List<int>();
        st3.Add(0 );
        st3.Add(0 );
        st3.Add(1 );
        for (int i = 0; i < 1000; i++)
            st3.Add(Mathf.FloorToInt((i + 10) * 2.5f));
        SaveAndLoad.Initialize(st3, generalFile);

    }

    //[System.Serializable]
    //public struct Nivel
    //{
    //    #region Info
    //    private string name;
    //    private string path;
    //    #endregion
    //    #region Level Setttings
    //    [SerializeField]
    //    private int numberOfLevels;
    //    [SerializeField]
    //    private List<int> clickPerLevel;
    //    #endregion

    //    #region Functions
    //    public void Initialize(int index)
    //    {
    //        index++;
    //        name = fileLevel + index.ToString() + ".json";
    //        path = Application.persistentDataPath + "/" + name;

    //        SaveAndLoad.Initialize(new List<string>(), path);

    //        List<string> st = SaveAndLoad.Read(path);
    //        clickPerLevel = new List<int>();

    //        foreach (string item in st)
    //        {
    //            int val;
    //            int.TryParse(item, out val);

    //            clickPerLevel.Add(val);
    //        }
    //        if (clickPerLevel != null)
    //            numberOfLevels = clickPerLevel.Count;
    //    }
    //    #endregion
    //}

    //#region File Names
    //private const string fileData = "data.json";
    //private const string fileLevel = "level_";
    //#endregion

    //#region Predet. General Data
    //private const int GATONEDAS = 0;
    //private const int GATONEDASPERCLICK = 1;
    //private const int LEVEL = 0;
    //private const int CLICKS = 0;
    //private const int NUNMBEROFLEVELS = 2;
    //#endregion
    //#region Predet. Level Data
    ////TODO Cambiar esta variable.
    //private const int LEVELBASIC = 10;
    //private const float TIME = 20F;
    //private const float PERCENT = 100f;
    //private const int NIVELMAXIMO = 50;
    //#endregion

    //[SerializeField]
    //private Nivel[] lista;
    //[SerializeField]
    //private List<int> costeMejora;



    //#region General Data
    //private int timesPressed;

    //[SerializeField]
    //private int gatonedas;
    //private int gatonedasPerClick;

    //private int levelScore;
    //#endregion
    //#region Level Temp. Data
    //private int level;
    //private int levelPercent;
    //private int levelPressed;

    //private float time;
    //private int timeLeft;

    //private float milkOriginalY;

    //private int perClickIndex;
    //#endregion

    //[Header("Textos Para que se Actualicen:")]
    //public Text timeText;
    //[Space(5)]
    //public Text levelText;
    //public Text levelPercentText;
    //public Text levelTextScore;
    //[Space(5)]
    //public Text gatonedaText;
    //public Text gatonedasPerClickText;
    //public Text costeMejoraText;
    //public Text noHaySuficienteGatonedas;


    //[Header("Objetos a Actualizar:")]
    //public RectTransform milk;
    //[Space(5)]
    //public GameObject heart;
    //public Transform parentHeartLevel;
    //public Transform parentHeartMenu;
    //[Space(8)]
    //public List<GameObject> listHeartLevel;
    //public List<GameObject> listHeartMenu;


    //[Header("Events")]
    //public UnityEvent startGame;
    //public UnityEvent endGame;
    //public UnityEvent muyCaro;

    //void Start()
    //{
    //    #region Inicializar lista de niveles
    //    lista = new Nivel[NUNMBEROFLEVELS];
    //    for (int i = 0; i < lista.Length; i++)
    //        lista[i].Initialize(i);
    //    #endregion

    //    #region Inicializar data general
    //    List<string> data = new List<string>();
    //    data.Add(GATONEDAS.ToString());
    //    data.Add(GATONEDASPERCLICK.ToString());
    //    data.Add(LEVEL.ToString());
    //    data.Add(CLICKS.ToString());

    //    SaveAndLoad.Initialize(data, fileData);

    //    // Inicializar el resto de variables.
    //    time = Time.time;
    //    levelPercent = 0;
    //    levelPressed = 0;
    //    timeLeft = -1;
    //    milkOriginalY = milk.rect.height;

    //    // Carga en "file" el contenido del archivo.
    //    List<string> file = SaveAndLoad.Read(fileData);

    //    // Introduce los datos de "file" en las variables correspondientes.
    //    int.TryParse(file[0], out gatonedas);
    //    int.TryParse(file[1], out gatonedasPerClick);
    //    int.TryParse(file[2], out levelScore);
    //    int.TryParse(file[3], out timesPressed);
    //    int.TryParse(file[4], out perClickIndex);
    //    #endregion

    //    // Inicializa las particulas.
    //    instantiateParticle(levelScore, parentHeartMenu, ref listHeartMenu);
    //}




    //void Update()
    //{
    //    Level();
    //    updateText();
    //}

    ///// <summary>
    ///// Actualiza los datos del nivel. 
    ///// Y se encarga de las transiciones
    /////  de niveles.
    ///// </summary>
    //private void Level()
    //{
    //    // Pulsaciones necesarias para acabar el nivel.
    //    int levelClickRequired = level;
    //    // Se ha pulsado el boton suficientes veces
    //    // para pasar al siquiente nivel.
    //    bool levelClickAcomplish = levelPressed >= levelClickRequired;
    //    // Nos encontramos en el menu principal
    //    // (No se esta jugando).
    //    bool notMenu = level > 0;

    //    if(notMenu && timeLeft == 0)
    //        EndGame();


    //    // State: Cambiar de nivel
    //    if(notMenu && levelClickAcomplish)
    //    {
    //        levelEnded();
    //    }
    //    // State: Jugando el nivel.
    //    else if (notMenu)
    //    {
    //        // Actualiza el marcador 
    //        // de porcentaje del nivel.
    //        levelsPercent(levelClickRequired);

    //        // Actualiza lo necesario para
    //        // que la leche se encuentre en su posicion.
    //        milkSize();

    //        // Actualiza el tiempo que queda
    //        // para acabar el nivel.
    //        float floatTimeleft = TIME - (Time.time - time);
    //        timeLeft = Mathf.FloorToInt(floatTimeleft);
    //    }
    //}
    ///// <summary>
    ///// Actualiza "levelPercent" para que indique
    ///// el porcentaje actual del nivel.
    ///// </summary>
    ///// <param name="clicksRequire"> Clicks necesarios para acabar el nivel</param>
    //private void levelsPercent(int clicksRequire)
    //{
    //    // Calcula el factor que representa el 
    //    // porcentaje actual del nivel.
    //    float factor = (float)levelPressed / (float)clicksRequire;
    //    // Cambia el factor a valor de porciento.
    //    int percent = Mathf.FloorToInt(factor * PERCENT);

    //    // Asigna el porcentaje a "levelPercent".
    //    levelPercent = percent;
    //}
    ///// <summary>
    ///// Actualiza la escala de la leche
    ///// respecto al porcentaje del progreso
    ///// del nivel.
    ///// </summary>
    //private void milkSize()
    //{
    //    // Calcula la talla del objeto
    //    // usando el porcentaje del nivel
    //    // y el tamaño original del objeto.
    //    float sizeY = milkOriginalY / PERCENT;
    //    sizeY *= levelPercent;
    //    // Si el tamaño calculado sobrepasa
    //    // el original, se reasignara el
    //    // tamaño original.
    //    sizeY = sizeY < milkOriginalY ? sizeY : milkOriginalY;

    //    // Se asigna el valor de "sizeY" a la escala
    //    // del objeto.
    //    milk.sizeDelta = new Vector2(milk.sizeDelta.x, sizeY);
    //}
    ///// <summary>
    ///// Cambia al siguente nivel.
    ///// Si se ha llegado al limite de niveles acaba el juego;
    ///// </summary>
    //private void levelEnded()
    //{
    //    // Verifica que no se sobrepase
    //    // el nivel maximo.
    //    if (level == NIVELMAXIMO)
    //        EndGame();

    //    // Aumenta en 1 el nivel.
    //    level++;
    //    // Resetea el porcentaje.
    //    levelPercent = 0;
    //    // Resetea las pulsaciones del nivel.
    //    levelPressed = 0;
    //    // Resetea el tiempo;
    //    time = Time.time;

    //    // Crea un corazon.
    //    instantiateParticle(1, parentHeartLevel, ref listHeartLevel);
    //}

    ///// <summary>
    ///// Crea Corazones.
    ///// </summary>
    ///// <param name="amount"> Cantidad de corazones a crear.</param>
    ///// <param name="parent"> Transform que se usara como padre. </param>
    ///// <param name="list"> Lista donde se guardaran los corazones creados. </param>
    //private void instantiateParticle(int amount, Transform parent, ref List<GameObject> list)
    //{
    //    for(int i = 0; i < amount; i++)
    //    {
    //        GameObject obj = Instantiate(heart);
    //        obj.transform.parent = parent;
    //        obj.transform.position = parent.transform.position;
    //        obj.GetComponent<StartAnim>().time = Time.time + Random.Range(0.3f, 2f);
    //        list.Add(obj);
    //    }
    //}
    //private void updateText()
    //{
    //    // Monedas.
    //    string st = gatonedas.ToString("D9");
    //    gatonedaText.text = string.Format("{0}{1}{2}.{3}{4}{5}.{6}{7}{8}",
    //        st[0], st[1], st[2], st[3], st[4], st[5], st[6], st[7], st[8]);

    //    // Score.
    //    levelTextScore.text = "Level: " + levelScore.ToString("D2");

    //    // Coste mejora.
    //    string value = "";
    //    string letter = "";
    //    compressInt(ref value, ref letter);
    //    costeMejoraText.text = value + letter;
    //    // Gatonedas por click.
    //    gatonedasPerClickText.text = gatonedasPerClick.ToString("D2");

    //    if (level > 0)
    //    {
    //        // Porcentaje.
    //        levelPercentText.text = levelPercent + "%";
    //        // Tiempo.
    //        timeText.text = timeLeft.ToString("D2");
    //        // Nivel.
    //        levelText.text = "Level: " + level.ToString("D2");
    //    }
    //}

    //private void compressInt(ref string value, ref string letter)
    //{
    //    int unit = 1000;
    //    if (costeMejora[perClickIndex] < unit)
    //    {
    //        value = costeMejora[perClickIndex].ToString("D3");
    //        letter = " ";
    //        return;
    //    }
    //    int exp = (int)(Mathf.Log(costeMejora[perClickIndex]) / Mathf.Log(unit));
    //    letter = "kMGTPE"[exp - 1].ToString();
    //    int val = (int)(costeMejora[perClickIndex] / Mathf.Pow(unit, exp));
    //    value = val.ToString("D3");
    //}


    //public void pressed()
    //{
    //    timesPressed++;
    //    levelPressed += gatonedasPerClick;
    //    gatonedas += gatonedasPerClick;
    //}
    ///// <summary>
    ///// Guarda los datos actuales
    /////  y cierra la aplicacion.
    ///// </summary>
    //public void Quit()
    //{
    //    // Crea un lista donde se 
    //    // guardaran los datos actuales.
    //    List<string> st = new List<string>();
    //    st.Add(gatonedas.ToString());
    //    st.Add(gatonedasPerClick.ToString());
    //    st.Add(levelScore.ToString());
    //    st.Add(timesPressed.ToString());

    //    // Guarda los datos actuales.
    //    SaveAndLoad.Write(ref st, fileData);

    //    // Cierra la aplicacion.
    //    Application.Quit();
    //}
    ///// <summary>
    ///// Cambia la velocidad del juego.
    ///// </summary>
    ///// <param name="state">True: Estado normal.  
    /////  False: Pausa.</param>
    //public void IsRuning(bool state)
    //{
    //    switch (state)
    //    {
    //        case true:
    //            Time.timeScale = 1;
    //            break;
    //        case false:
    //            Time.timeScale = 0;
    //            break;
    //    }
    //}

    //public void StartGame()
    //{
    //    level = 1;
    //    levelPressed = 0;
    //    time = Time.time;
    //    startGame.Invoke();
    //}
    //public void EndGame()
    //{
    //    foreach (GameObject temp in listHeartMenu)
    //    {
    //        temp.transform.localScale = Vector3.up;
    //        temp.GetComponent<StartAnim>().t = false;
    //        temp.GetComponent<StartAnim>().time = Time.time + Random.Range(0.3f, 2f);
    //    }

    //    if (levelScore < level)
    //    {
    //        levelScore = level;

    //        for (int i = 0; i < level - levelScore; i++)
    //        {
    //            GameObject obj = Instantiate(heart);
    //            obj.transform.parent = parentHeartMenu;
    //            obj.transform.position = parentHeartMenu.transform.position;
    //            obj.GetComponent<StartAnim>().time = Time.time + Random.Range(0.3f, 2f);
    //            listHeartMenu.Add(obj);
    //        }
    //    }

    //    foreach (GameObject del in listHeartLevel)
    //        GameObject.Destroy(del);

    //    level = 0;
    //    timeLeft = -1;
    //    levelPressed = 0;
    //    levelPercent = 0;
    //    listHeartLevel.Clear();
    //    endGame.Invoke();
    //}
    //public void IncreaseGatonedaPerClick()
    //{
    //    if (gatonedas < costeMejora[perClickIndex])
    //    {

    //        muyCaro.Invoke();
    //        noHaySuficienteGatonedas.text = costeMejora[perClickIndex].ToString("D9");
    //        return;
    //    }

    //    gatonedas -= costeMejora[perClickIndex];
    //    gatonedasPerClick += 5;
    //    perClickIndex++;
    //}
}
