//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class ChangeColor : MonoBehaviour
//{
//    public GameObject model;
//    public Color color;
//    //Start is called before the first frame update
//    private void Start()
//    {

//    }

//    //Update is called once per frame
//    void Update()
//    {

//    }
//    public void ChangeColor_BTN()
//    {
//        model.GetComponent<Renderer>().material.color = color;
//    }
//}

using System.Collections.Generic;
using UnityEngine;

public class RandomModelToggle : MonoBehaviour
{
    [Header("Si dejas vacío 'models', se llenará automáticamente con los hijos de este GameObject")]
    public GameObject[] models;

    [Tooltip("Si true, buscará recursivamente en todos los descendientes (no sólo hijos directos).")]
    public bool includeChildrenRecursively = false;

    [Tooltip("Si true, incluye también objetos inactivos al recopilar modelos.")]
    public bool includeInactiveWhenCollecting = true;

    [Tooltip("Si true, evita elegir el mismo modelo dos veces seguidas (si hay >1).")]
    public bool avoidSameAsPrevious = true;

    private int currentIndex = -1;

    private void Start()
    {
        if (models == null || models.Length == 0)
        {
            CollectModelsFromChildren();
        }

        for (int i = 0; i < models.Length; i++)
        {
            if (models[i] != null)
                models[i].SetActive(false);
        }
    }

    public void ShowRandomExistingModel()
    {
        if (models == null || models.Length == 0)
        {
            Debug.LogWarning("RandomModelToggle: no hay modelos en el array 'models'.");
            return;
        }

        int idx = Random.Range(0, models.Length);

        if (avoidSameAsPrevious && models.Length > 1)
        {
            // evitar elegir el mismo índice
            int attempts = 0;
            while (idx == currentIndex && attempts < 10)
            {
                idx = Random.Range(0, models.Length);
                attempts++;
            }
        }

        // activar el elegido y desactivar los demás
        for (int i = 0; i < models.Length; i++)
        {
            if (models[i] != null)
                models[i].SetActive(i == idx);
        }

        currentIndex = idx;
    }

    private void CollectModelsFromChildren()
    {
        List<GameObject> list = new List<GameObject>();

        if (includeChildrenRecursively)
        {
            // incluye descendientes recursivamente
            Transform[] transforms = GetComponentsInChildren<Transform>(includeInactiveWhenCollecting);
            foreach (Transform t in transforms)
            {
                if (t == this.transform) continue; // saltar el mismo objeto
                if (t.GetComponent<Canvas>() != null) continue;
                list.Add(t.gameObject);
            }
        }
        else
        {
            // sólo hijos directos
            foreach (Transform child in transform)
            {
                if (!includeInactiveWhenCollecting && !child.gameObject.activeSelf) continue;
                if (child.GetComponent<Canvas>() != null) continue; // omitir UI si la hubiera
                list.Add(child.gameObject);
            }
        }

        models = list.ToArray();
    }
}

