using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureAI : MonoBehaviour
{
    [Header("Main Properties")]
    public Ecosystem fatherReference;
    public int timeOfEachGen = 2;//2 seconds per generation
    public float FinalFitness = 0;//fitness at the end of simulation
    public float Resilience = 1f;//0 up to 2
    public float Size = 1f;//0.5 up to 1.5 (0.5 is the fixed constant equal 0 of variation)
    public float Speed = 1f;//3.5 up to 5.5 (3.5 is the fixed constant equal 0 of variation)
    public float FView = 1f;//5 up to 7 (5 is the fixed constant equal 0 of variation)
    public float Energy = 5f;//max energy;
    public float MaxEarsHigh = 0.23f;// the highes the ears can take as the speed of the creature
    public Color ColorOfMaxResilience;
    [HideInInspector] public float timeToChargeEnergyCost = 0.5f;
    public float[] Genoma = new float[4];// [2f,2f,2f,2f];
    [SerializeField] int food = 0;
    [Header("Fenotype")]
    public int AmIAPredator = 0;
    public GameObject PredaterFangs;
    public GameObject Ears;
    public GameObject Eyes;
    [Header("Parents Data")]// [0 up to 3] is normal genes, [4] is the number id of the father 
    public float myId;//creature ID
    public int MutationId = -1;// ID of gene mutated in this creature (-1 = None)
    public float[] ParentA_GenomaNFatherID = new float[5];
    public float[] ParentB_GenomaNFatherID = new float[5];
    //e(x) = T amanho3 + V elocidade2 + V isao
    //f(x) = (comida coletada) + (resiliencia ˆ ) + e(x)/14
    [Header("Other")]
    public GameObject FieldViewArea;
    public Movement movement;
    public List<Renderer> materialsToChange;
    public Material DeadMat;
    public float LerpValue(float maxInputV, float maxBigV, float bigV2Insert)// thats for take a small number (ex 0.7) from a big input value referred to the value of the real game adapation variable (0to15)
    {//(1,100,20) = 0,05
        float percentInsert = bigV2Insert * 100;
        percentInsert = percentInsert / maxBigV;
        return maxInputV / percentInsert;
    }
    public float LerpBackValue(float maxInputV, float maxBigV, float VInput2Insert)// thats for take a big number (ex 15) from a small input value referred to the value of the input variable (0to2)
    {//(1,100,0.05) = 20
        float percentInsert = VInput2Insert * 100;
        percentInsert = percentInsert / maxInputV;
        return maxBigV / percentInsert;
    }
    public float fitness()//calculate the fitness 
    {// calculate fitness
        FinalFitness = (food) + (Resilience) + energy() / 14;
        return FinalFitness;//14 is referred to the energy() maximum value
    }
    float energy()// calculate energy cost (doubled if predator)
    {// calculate energy cost
        if (AmIAPredator == 1) {// predators consumes the double of energy
            return (Mathf.Pow(Size, 3) + Mathf.Pow(Speed, 2) + FView)*2;
        }
        return Mathf.Pow(Size, 3) + Mathf.Pow(Speed, 2) + FView;
    }


    public void UpdatePropertiesByNewGenoma()//this is called to update stats after receivein a new genoma from father
    {
        Resilience = Genoma[0]; Size = Genoma[1];
        Speed = Genoma[2]; FView = Genoma[3];
    }
    #region Fenotype 
    public void ManifestFenotype()// call all fenotype functions
    {
        SetColorByResilience(); GrowEarsBySpeed(); SetFangs();
        SetSpeed(); SetSize(); SetEyeSize();
    }
    void SetColorByResilience()// set color of the creature by gene
    {
        ColorOfMaxResilience = fatherReference.ColorBiggest;
        float Rvalue = Mathf.Lerp(materialsToChange[0].material.color.r, ColorOfMaxResilience.r, LerpValue(fatherReference.Resilience_minmax_Input[1], ColorOfMaxResilience.r, Mathf.Clamp(Resilience, 0, 2)));
        float Gvalue = Mathf.Lerp(materialsToChange[0].material.color.g, ColorOfMaxResilience.g, LerpValue(fatherReference.Resilience_minmax_Input[1], ColorOfMaxResilience.g, Mathf.Clamp(Resilience, 0, 2)));
        float Bvalue = Mathf.Lerp(materialsToChange[0].material.color.b, ColorOfMaxResilience.b, LerpValue(fatherReference.Resilience_minmax_Input[1], ColorOfMaxResilience.b, Mathf.Clamp(Resilience, 0, 2)));
        foreach (Renderer r in materialsToChange)
        {
            r.material.color = new Color(Rvalue, Gvalue, Bvalue, 255);
        }
    }
    void GrowEarsBySpeed()//0.3 to 0.4150001 over gene
    {
        Ears.transform.position =new Vector3(transform.position.x, 
            Mathf.Clamp((LerpValue(fatherReference.Speed_minmax_Input[1], /*(MaxEarsHigh)*/ 415f, Speed))/1000,/*fatherReference.Speed_minmax_Input[0]*/ 0.3f,/*fatherReference.Speed_minmax_Input[1]*/ 0.4150001f), 
            transform.position.z);
    }
    void SetFangs()// if not a predator hide the fangs over gene
    {
        if (AmIAPredator==0) { PredaterFangs.gameObject.SetActive(false); }
    }
    void SetSpeed()// set speed over gene
    {
        float v = Mathf.Clamp(LerpValue(fatherReference.Speed_minmax_Input[1],
            fatherReference.Speed_minmax_BigInput[1],Speed),
            fatherReference.Speed_minmax_BigInput[0],
            fatherReference.Speed_minmax_BigInput[1]);
        movement.SetSpeed(v);
    }
    void SetSize()//0.3 up to 0.5 over gene
    {
        float s = Mathf.Clamp((LerpValue(fatherReference.Size_roo_minmax_Input[1], 
            fatherReference.Size_roo_minmax_BigInput[1], Size)), 
            fatherReference.Size_roo_minmax_BigInput[0], 
            fatherReference.Size_roo_minmax_BigInput[1]);
        transform.localScale =  new Vector3(s,s,s);
    }
    void SetEyeSize()//0.3 up to 0.5 over gene
    {
        float f = Mathf.Clamp((LerpValue(fatherReference.FView_minmax_Input[1],
            fatherReference.FView_minmax_BigInput[1], FView)),
            fatherReference.FView_minmax_BigInput[0],
            fatherReference.FView_minmax_BigInput[1]);
        Eyes.transform.localScale = new Vector3(f, f, Mathf.Clamp((f-0.2f),1,1.2f));
    }
    #endregion

    public void EatFood(GameObject foodtoeat)// eat the food after winning it in a fight
    {
        if (food < 5) { food++; Destroy(foodtoeat); }
        else { Stop(); }
    }
    public void EatCreature(GameObject foodtoeat)// eats other creature when its prey and predator
    {
        if (food < 5) { food++; foodtoeat.GetComponent<CreatureAI>().Eaten(); }
        else { Stop(); }
    }
    public void Eaten()// color it red when its eaten
    {
        if (movement != null)
        {
            Stop();
            Energy = 0;
            foreach(Renderer r in materialsToChange)
            {
                r.material = DeadMat;
            }
        }
    }
    private void Stop()// stops the creature
    {
        movement.imOn = false;
        movement.StopAgent();
    }
    public void setMyID(int id)// setup the ide on the tree
    {
        myId = id;
    }
    void Awake()
    {
        movement = GetComponent<Movement>();
        StartCoroutine("EnergyCost");
        //test();
    }
    private void test()
    {
        //Debug.Log(Eyes.transform.localScale);
    }
    IEnumerator EnergyCost()// this calculates energy cost over time
    {
        yield return new WaitForSeconds(timeToChargeEnergyCost);
        Energy -= energy(); 
        if (Energy > 0) { StartCoroutine("EnergyCost");  }
        else { Stop(); }
    }
    private void OnCollisionEnter(Collision collision)// used to detect proximity of food or preys
    {
        if (collision.gameObject.CompareTag("Food"))
        {
            if (collision.gameObject.GetComponent<Food>().CreatureCandidate == this)
            { EatFood(collision.gameObject); }
        }
        if (  AmIAPredator==1 && collision.gameObject.CompareTag("Creature"))
        {
            if(movement!=null)
            {
                if (movement.imOn == true)
                {
                    EatCreature(collision.gameObject);
                }
            }
            
        }
    }
    private void OnCollisionStay(Collision collision)// used to detect proximity of food or preys
    {
        if (collision.gameObject.CompareTag("Food"))
        {
            if (collision.gameObject.GetComponent<Food>().CreatureCandidate == this)
            { EatFood(collision.gameObject); }
        }
        if (AmIAPredator == 1 && collision.gameObject.CompareTag("Creature"))
        {
            if (movement != null)
            {
                if (movement.imOn == true)
                {
                    EatCreature(collision.gameObject);
                }
            }

        }
    }

}
