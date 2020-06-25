using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ecosystem : MonoBehaviour
{
    [Header("Amount Values")]
    public int Crossover = 90;// means that theres 90% of chance to crossover
    public int Mutation = 5;// means thath each gene has 5% of chance to mutate
    public int EvolutionPression = 10;//means that each 10 unities of best creatures will survive
    public int numGenerations = 200;// how much generations will be generated
    public string IterationPupulation = "1";//index of numGenerations
    public int dataPath = 1;//index of data simulation path (englobes the numGenerations)
    public int maxFoodRange = 50;// how much food unities will be generate by generation
    public int maxCreaRange = 100;// how much creatures will be created per generation
    public bool Predators = false;
    public int PredatorsPercentage = 40;
    [SerializeField] List<CreatureAI> Creatures;

    [Header("Ranges Min&Max Inputs")]// these are the input manipulated by the cromossome they're also a scale to the BigInput output
    public float[] Resilience_minmax_Input = new float[2] {0,2};//on BigInput is 5 up to 7
    public float[] Size_roo_minmax_Input = new float[2]   {0,2};//on BigInput is 0.5 up to 1.5
    public float[] Speed_minmax_Input =   new float[2]    {0,2};//on BigInput is 3.5 up to 5.5
    public float[] FView_minmax_Input =   new float[2]    {0,2};//on BigInput is 5 up to 7
    public Color ColorBiggest = new Color();//(154f, 245f, 131, 255);

    [Header("Ranges Min&Max Inputs")]// these are the translacations in scale of the small Inputs above, wich means that if [0] of the small Input means the [0] of the BigInput
    public float[] Resilience_minmax_BigInput = new float[2] { 5, 7 };
    public float[] Size_roo_minmax_BigInput = new float[2]   {0.1f, 0.3f};
    public float[] Speed_minmax_BigInput = new float[2]      { 3.5f, 5.5f };
    public float[] FView_minmax_BigInput = new float[2]      { 1, 1.4f };

    [Header("References")]
    public List<Transform> GridList = new List<Transform>();
    public List<CreatureAI> SelectedCreatures = new List<CreatureAI>();
    public List<CreatureAI> LastElitizedCreatures = new List<CreatureAI>();
    public GameObject foodPrefab;
    public GameObject creaPrefab;
    public GameObject foodHolder;
    public GameObject creaHolder;

    private void Awake()
    {
        Init();

    }
    private void Init()
    {
        for (int i = 0; i < maxFoodRange; i++) { foodGenerate(); }
        for (int i = 0; i < maxCreaRange; i++) { creaGenerate(); }
        if (HandleTextFile.DetectGenerationLine("aux") != "0")
        {
            IterationPupulation = (HandleTextFile.DetectGenerationLine("aux")); bool ended = false;
            if (HandleTextFile.DetectGenerationLine("aux") != numGenerations.ToString())
            {
                int value = int.Parse(IterationPupulation);
                value++;
                IterationPupulation = value.ToString();
            }
            else { ended = true; }


            //UpdateHud();
            //DecryptElitizedLastGeneration();
            for (int i = 0; i < 45; i++)
            {
                CreatureAI[] cc = CrossOver();
                foreach (CreatureAI c in cc)
                {
                    creaGenerateByHeritance(c);
                }
            }
            if (ended == false)
            {
                HandleTextFile.ClearAux("aux");
                Invoke("WriteGeneration", 10f);
            }


        }
        else
        {
            Invoke("WriteGeneration", 10f);


        }

    }
    void foodGenerate()
    {
        int aim = Random.Range(0, GridList.Count - 1);
        GameObject f = Instantiate(foodPrefab, GridList[aim].transform.position , Quaternion.identity) as GameObject;
        f.transform.position += new Vector3(0,0.3f,0);
        f.transform.parent = foodHolder.transform;
    }
    void creaGenerate()//generate creature first generation
    {
        int aim = Random.Range(0, GridList.Count - 1);
        GameObject f = Instantiate(creaPrefab, GridList[aim].transform.position, Quaternion.identity) as GameObject;
        f.transform.position += new Vector3(0, 0.3f, 0);
        f.transform.parent = creaHolder.transform;
        f.GetComponent<Movement>().gridpoints = this;
        f.GetComponent<Movement>().imOn=true;
        CreatureAI brain = f.GetComponent<CreatureAI>();
        brain.Genoma = generateGenes();//<-
        Creatures.Add(brain);
        brain.fatherReference = this;//<-
        brain.UpdatePropertiesByNewGenoma();//<-
        if (Predators){
            if (Random.Range(1, 100) < PredatorsPercentage)
            {
                brain.AmIAPredator = 1;
            }
        }
        brain.ManifestFenotype();
        brain.setMyID(Creatures.Count-1);
        
    }
    void creaGenerateByHeritance(CreatureAI c)//generate creature
    {
        int aim = Random.Range(0, GridList.Count - 1);
        GameObject f = Instantiate(creaPrefab, GridList[aim].transform.position, Quaternion.identity) as GameObject;
        f.transform.position += new Vector3(0, 0.3f, 0);
        f.transform.parent = creaHolder.transform;
        f.GetComponent<Movement>().gridpoints = this;
        f.GetComponent<Movement>().imOn = true;
        CreatureAI brain = f.GetComponent<CreatureAI>();
        //brain.Genoma = c.Genoma;//<-
        brain = c;
        Creatures.Add(brain);
        brain.fatherReference = this;
        brain.UpdatePropertiesByNewGenoma();
        if (Predators)
        {
            if (Random.Range(1, 100) < PredatorsPercentage)
            {
                brain.AmIAPredator = 1;
            }
        }
        brain.ManifestFenotype();
        brain.setMyID(Creatures.Count - 1);

    }
    float randomFloatGene()
    {
        return Random.Range(0.0f, 2.0f);
    }
    float[]  generateGenes()//generate random values on first generation
    {
        float Resilience_generated = Random.Range(0.0f,2.0f);
        float Size_roo_generated = Random.Range(0.0f, 2.0f);//0.5 a 1.5
        float Speed_generated = Random.Range(0.0f, 2.0f);//3.5 
        float FView_generated = Random.Range(0.0f, 2.0f);//2 a 5
        
        float[] Genoma_generated = new float[4];
        CreatureAI CreatureBrain = new CreatureAI();
        Genoma_generated[0] = Mathf.Clamp(CreatureBrain.LerpValue((Resilience_minmax_Input[1] - Resilience_minmax_Input[0]), Resilience_minmax_BigInput[1] - Resilience_minmax_BigInput[0], Resilience_generated),Resilience_minmax_Input[0], Resilience_minmax_Input[1]);
        Genoma_generated[1] = Mathf.Clamp(CreatureBrain.LerpValue((Size_roo_minmax_Input[1] - Size_roo_minmax_Input[0]), Size_roo_minmax_BigInput[1] - Size_roo_minmax_BigInput[0], Size_roo_generated),Size_roo_minmax_Input[0],Size_roo_minmax_Input[1]);
        Genoma_generated[2] = Mathf.Clamp(CreatureBrain.LerpValue((Speed_minmax_Input[1] - Speed_minmax_Input[0]), Speed_minmax_BigInput[1] - Speed_minmax_BigInput[0], Speed_generated),Speed_minmax_Input[0], Speed_minmax_Input[1]);
        Genoma_generated[3] = Mathf.Clamp(CreatureBrain.LerpValue((FView_minmax_Input[1] - FView_minmax_Input[0]), FView_minmax_BigInput[1] - FView_minmax_BigInput[0], FView_generated),FView_minmax_Input[0],FView_minmax_Input[1]);
        return Genoma_generated;
    }
    void WriteDetails()
    {
        if (IterationPupulation == "1")
        {
            HandleTextFile.WriteString("Experiment Nº: " + dataPath+", Crossover: "+ Crossover+ ", Mutation: "+ Mutation+ ", EvolutionPression: "+ EvolutionPression+ ", numGenerations: "+ numGenerations+", MinMax = [0f-2f], Food: "+ maxFoodRange+", Number Creatures: "+ maxCreaRange+", Predators?: "+Predators+", Predators%: "+PredatorsPercentage, IterationPupulation.ToString(), true);
           
        }
    }
    void WriteGeneration()
    {
        HandleTextFile.WriteString("Generation: " + IterationPupulation, IterationPupulation.ToString(), true);
        EvaluateAllFitness(); ElitistSelection();
        for (int i = 0; i < 25; i++)
        {
            if (Creatures[i] != null)
                HandleTextFile.WriteString(WriteACreature(Creatures[i]), IterationPupulation.ToString(), true);
        }
        for (int i = 25; i < 50; i++)
        {
            if (Creatures[i] != null)
                HandleTextFile.WriteString(WriteACreature(Creatures[i]), IterationPupulation.ToString(), true);
        }
        for (int i = 50; i < 75; i++)
        {
            if (Creatures[i] != null)
                HandleTextFile.WriteString(WriteACreature(Creatures[i]), IterationPupulation.ToString(), true);
        }
        for (int i = 75; i < 100; i++)
        {
            if(Creatures[i]!=null)
                HandleTextFile.WriteString(WriteACreature(Creatures[i]), IterationPupulation.ToString(), true);
        }
        //-----Elitism Writing
        HandleTextFile.WriteString("Selected By Elitism Gen: " + IterationPupulation, IterationPupulation.ToString(), true);
        for (int i = 0; i < EvolutionPression; i++)
        {
            if (SelectedCreatures[i] != null)
                HandleTextFile.WriteString(WriteACreature(SelectedCreatures[i]), IterationPupulation.ToString(), true);
        }
        HandleTextFile.WriteString("-------------------------------------------------------------------------------------------------------------------" , IterationPupulation.ToString(), true);
        for (int i = 0; i < EvolutionPression; i++)
        {
            if (SelectedCreatures[i] != null)
                HandleTextFile.WriteString(WriteACreature(SelectedCreatures[i]), "aux", true);
        }
        HandleTextFile.WriteString(IterationPupulation.ToString(),"aux",true);
        HandleTextFile.WriteString("G", "aux", true);
        Debug.Log("Restarting scene... Forward to next generation in 10s");

        Invoke("restartScene", 10f);
    }
    void restartScene()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        DestroyDiscarted();
        Init();
    }
    void DestroyDiscarted()
    {
        foreach(CreatureAI c in Creatures)
        {
            if (!SelectedCreatures.Contains(c))
            {
                Destroy(c.gameObject);
            }
        }
    }
    string WriteACreature(CreatureAI c)
    {/////////////////////_0_/////_1_///_2_/////_3_/////////////_4_///////////////_5_///////////_6_////////_7_/////_8_/////////////_9_//////_10_////////////////////////_11_////_12_////////////////////////_13_///_14_///////////_15_///_16_////////_17
        string towrite = "ID:_"+c.myId+"_[_"+c.Genoma[0]+"_"+c.Genoma[1]+"_"+c.Genoma[2]+"_"+c.Genoma[3]+"_]-P:_"+c.AmIAPredator+"_Pa:_"+c.ParentA_GenomaNFatherID[4]+"_Pb:_"+c.ParentB_GenomaNFatherID[4]+"_F:_"+c.FinalFitness+"_M:_"+c.MutationId+"_ ;";
        return towrite;
    }
    void DecryptElitizedLastGeneration()
    {
        HandleTextFile.ReadString("aux");
        string[] objects = HandleTextFile.Data.Split(';');
        string[,] characteristics = new string[objects.Length, 18];
        //Debug.Log("Objectslenght "+objects.Length);
        for (int j = 0; j < objects.Length-1; j++)
        {
            string[] temp = objects[j].Split('_');
            //Debug.Log("temp "+temp.Length);
            for (int i = 0; i < 18; i++)
            {
                //Debug.Log("char " + j + "," + i);
                characteristics[j, i] = temp[i];
            }
        }
        for(int j =0; j < objects.Length-1; j++)
        {
            CreatureAI c = new CreatureAI();
            c.myId = int.Parse(characteristics[j, 1]);
            c.Genoma[0] = float.Parse(characteristics[j, 3]);
            c.Genoma[1] = float.Parse(characteristics[j, 4]);
            c.Genoma[2] = float.Parse(characteristics[j, 5]);
            c.Genoma[3] = float.Parse(characteristics[j, 6]);
            c.AmIAPredator = int.Parse(characteristics[j, 8]);
            c.FinalFitness = float.Parse(characteristics[j, 14]);

            //creaturesGlobalHeritance.creature.Add(c);
            LastElitizedCreatures.Add(c);

        }
        
    }

    CreatureAI[] CrossOver()//2 childs each time, played 5 times * 10
    {
        if (Random.Range(0, 100) <= Crossover)// will crossover
        {
            int seed = (Random.Range(0, 3));//wich genes to cross by array index randomized
            int indexParentA = Random.Range(0, LastElitizedCreatures.Count);
            int indexParentB = Random.Range(0, LastElitizedCreatures.Count);

            CreatureAI c = new CreatureAI();

            genomaSwiftAndMutation(LastElitizedCreatures[indexParentA].Genoma, LastElitizedCreatures[indexParentB].Genoma, seed);
            c.Genoma = outaA;

            c.ParentA_GenomaNFatherID[0] = LastElitizedCreatures[indexParentA].Genoma[0];
            c.ParentA_GenomaNFatherID[1] = LastElitizedCreatures[indexParentA].Genoma[1];
            c.ParentA_GenomaNFatherID[2] = LastElitizedCreatures[indexParentA].Genoma[2];
            c.ParentA_GenomaNFatherID[3] = LastElitizedCreatures[indexParentA].Genoma[3];
            c.ParentA_GenomaNFatherID[4] = LastElitizedCreatures[indexParentA].myId;

            c.ParentB_GenomaNFatherID[0] = LastElitizedCreatures[indexParentB].Genoma[0];
            c.ParentB_GenomaNFatherID[1] = LastElitizedCreatures[indexParentB].Genoma[1];
            c.ParentB_GenomaNFatherID[2] = LastElitizedCreatures[indexParentB].Genoma[2];
            c.ParentB_GenomaNFatherID[3] = LastElitizedCreatures[indexParentB].Genoma[3];
            c.ParentB_GenomaNFatherID[4] = LastElitizedCreatures[indexParentB].myId;

            CreatureAI d = new CreatureAI();
            d.Genoma = outaB;

            d.ParentA_GenomaNFatherID[0] = LastElitizedCreatures[indexParentA].Genoma[0];
            d.ParentA_GenomaNFatherID[1] = LastElitizedCreatures[indexParentA].Genoma[1];
            d.ParentA_GenomaNFatherID[2] = LastElitizedCreatures[indexParentA].Genoma[2];
            d.ParentA_GenomaNFatherID[3] = LastElitizedCreatures[indexParentA].Genoma[3];
            d.ParentA_GenomaNFatherID[4] = LastElitizedCreatures[indexParentA].myId;

            d.ParentB_GenomaNFatherID[0] = LastElitizedCreatures[indexParentB].Genoma[0];
            d.ParentB_GenomaNFatherID[1] = LastElitizedCreatures[indexParentB].Genoma[1];
            d.ParentB_GenomaNFatherID[2] = LastElitizedCreatures[indexParentB].Genoma[2];
            d.ParentB_GenomaNFatherID[3] = LastElitizedCreatures[indexParentB].Genoma[3];
            d.ParentB_GenomaNFatherID[4] = LastElitizedCreatures[indexParentB].myId;

            if(Mai != -1) { c.MutationId = Mai; c.Genoma[Mai] = Ma;  }
            if(Mbi != -1) { d.MutationId = Mbi; d.Genoma[Mbi] = Mb; }

            CreatureAI[] children = new CreatureAI[2];
            children[0] = c; children[1] = d;
            return children;
        }
        else
        {
            int seed = (Random.Range(0, 3));//wich genes to cross by array index randomized
            int indexParentA = Random.Range(0, LastElitizedCreatures.Count);
            ///int indexParentB = Random.Range(0, LastElitizedCreatures.Count);

            CreatureAI[] children = new CreatureAI[2];
            CreatureAI c = new CreatureAI();

            genomaSwiftAndMutation(LastElitizedCreatures[indexParentA].Genoma, LastElitizedCreatures[indexParentA].Genoma, seed);
            c.Genoma = outaA;

            c.ParentA_GenomaNFatherID[0] = LastElitizedCreatures[indexParentA].Genoma[0];
            c.ParentA_GenomaNFatherID[1] = LastElitizedCreatures[indexParentA].Genoma[1];
            c.ParentA_GenomaNFatherID[2] = LastElitizedCreatures[indexParentA].Genoma[2];
            c.ParentA_GenomaNFatherID[3] = LastElitizedCreatures[indexParentA].Genoma[3];
            c.ParentA_GenomaNFatherID[4] = LastElitizedCreatures[indexParentA].myId;

            c.ParentB_GenomaNFatherID[0] = LastElitizedCreatures[indexParentA].Genoma[0];
            c.ParentB_GenomaNFatherID[1] = LastElitizedCreatures[indexParentA].Genoma[1];
            c.ParentB_GenomaNFatherID[2] = LastElitizedCreatures[indexParentA].Genoma[2];
            c.ParentB_GenomaNFatherID[3] = LastElitizedCreatures[indexParentA].Genoma[3];
            c.ParentB_GenomaNFatherID[4] = LastElitizedCreatures[indexParentA].myId;

            CreatureAI d = new CreatureAI();
            d.Genoma = outaB;

            d.ParentA_GenomaNFatherID[0] = LastElitizedCreatures[indexParentA].Genoma[0];
            d.ParentA_GenomaNFatherID[1] = LastElitizedCreatures[indexParentA].Genoma[1];
            d.ParentA_GenomaNFatherID[2] = LastElitizedCreatures[indexParentA].Genoma[2];
            d.ParentA_GenomaNFatherID[3] = LastElitizedCreatures[indexParentA].Genoma[3];
            d.ParentA_GenomaNFatherID[4] = LastElitizedCreatures[indexParentA].myId;

            d.ParentB_GenomaNFatherID[0] = LastElitizedCreatures[indexParentA].Genoma[0];
            d.ParentB_GenomaNFatherID[1] = LastElitizedCreatures[indexParentA].Genoma[1];
            d.ParentB_GenomaNFatherID[2] = LastElitizedCreatures[indexParentA].Genoma[2];
            d.ParentB_GenomaNFatherID[3] = LastElitizedCreatures[indexParentA].Genoma[3];
            d.ParentB_GenomaNFatherID[4] = LastElitizedCreatures[indexParentA].myId;

            if (Mai != -1) { c.MutationId = Mai; c.Genoma[Mai] = Ma; }
            if (Mbi != -1) { d.MutationId = Mbi; d.Genoma[Mbi] = Mb; }

            children[0] = c; children[1] = d;
            return children; 
        }
    }
    float[] outaA; float[] outaB;
    float Ma; float Mb; int Mai=-1; int Mbi=-1;
    void genomaSwiftAndMutation(float[] genesA, float[] genesB, int indexer)
    {
        //float[,] thirdGenes = new float[genesA.Length, 2];
        //float[] fourthGenes = new float[genesA.Length];
        Mai = -1; Mbi = -1;
        float[] auxiliar = new float[genesA.Length];

        for(int i = indexer; i < genesA.Length; i++)//swift to auxiliar
        {
            auxiliar[i] = genesA[i];
        }
        for (int i = indexer; i < genesA.Length; i++)//swift exchange
        {
            genesA[i] = genesB[i];
            genesB[i] = auxiliar[i];
        }
        /*for(int i = 0; i < genesA.Length; i++)// filling the matrix
        {
            thirdGenes[i, 0] = genesA[i];
            thirdGenes[i, 1] = genesB[i];
        }
        return thirdGenes;*/
        #region Mutation
        if (Random.Range(0, 100) < Mutation)
        {
            indexer = Random.Range(0, 3);
            Mai = indexer; Ma = randomFloatGene();
        }
        if (Random.Range(0, 100) < Mutation)
        {
            indexer = Random.Range(0, 3);
            Mbi = indexer; Mb = randomFloatGene();
        }
        #endregion

        outaA = genesA; outaB = genesB;
    }
    void EvaluateAllFitness()
    {
        foreach (CreatureAI creature in Creatures)
        {
            creature.fitness();
        }
    }
    void ElitistSelection()
    {
        List<CreatureAI> tmp = new List<CreatureAI>();
        tmp = bubbleSort(Creatures);
        for (int i = tmp.Count-1; i > tmp.Count-1-EvolutionPression; i--)// iterates the list from the end up to the range backwards
        {
            SelectedCreatures.Add(tmp[i]);
        }

    }
    public static List<CreatureAI> bubbleSort(List<CreatureAI> vetor)
    {
        int tamanho = vetor.Count;
        int comparacoes = 0;
        int trocas = 0;

        for (int i = tamanho - 1; i >= 1; i--)
        {
            for (int j = 0; j < i; j++)
            {
                comparacoes++;
                if (vetor[j].FinalFitness > vetor[j + 1].FinalFitness)
                {
                    CreatureAI aux = vetor[j];
                    vetor[j] = vetor[j + 1];
                    vetor[j + 1] = aux;
                    trocas++;
                }
            }
        }

        return vetor;
    }

}
