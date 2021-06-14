using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour  
{
    public static CameraController Instance { get; private set; }

    private float shakeTimeRemaining, shakePower, shakeFadeTime;
    //public GameObject[] gameSlots;
    private Vector3 position;
    //private MasterEventListener masterEventListener;

    private void Awake()
    {
        Instance = this;
        //masterEventListener = GameObject.FindGameObjectWithTag("Player").GetComponent<MasterEventListener>();
    }

    private void OnEnable()
    {
        //gameSlots = GameObject.FindGameObjectsWithTag("Container");
        //for (int i = 0; i < gameSlots.Length; i++)
        //{
        //    gameSlots[i].GetComponent<GameSlots>().OnDropped += OnGameItemAdded;
        //}

        MasterEventListener.Instance.OnMerged += StartShake;
    }

    private void OnDisable()
    {
        //gameSlots = GameObject.FindGameObjectsWithTag("Container");
        //for (int i = 0; i < gameSlots.Length; i++)
        //{
        //    gameSlots[i].GetComponent<GameSlots>().OnDropped -= OnGameItemAdded;
        //}

        MasterEventListener.Instance.OnMerged -= StartShake;

    }

    void Start()
    {
        position = transform.position;
    }

    void Update()
    {
        
    }

    private void LateUpdate()
    {
        if (shakeTimeRemaining > 0)
        {
            shakeTimeRemaining -= Time.deltaTime;

            float xAmount = Random.Range(-.5f, .5f) * shakePower  ;
            float yAmount = Random.Range(-.5f, .5f) * shakePower  ;

            // bu enumerator ile daha iyi olacak

            transform.position = position;

            transform.position += new Vector3(xAmount, yAmount, 0);

            shakePower = Mathf.MoveTowards(shakePower, 0f, shakeFadeTime * Time.deltaTime);
        }

        //transform.position = position;
    }

    //private void OnGameItemAdded(object sender, GameSlots.OnDroppedEventHandler e)
    //{
    //    e.gameItem.OnMerged += StartShake;

    //}

    private void StartShake(object sender, GameItems.OnMergedEventArgs e)
    {
        shakeTimeRemaining = e.itemLevel;
        shakePower = e.itemLevel;

        shakeFadeTime = e.itemLevel / e.itemLevel *2;
    }
}
