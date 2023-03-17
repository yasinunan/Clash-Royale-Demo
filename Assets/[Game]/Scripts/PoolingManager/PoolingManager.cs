using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

namespace UOP1.Helper
{
    [Serializable]
    public class PoolObject
    {
        public string strObjectGroup;
        public string strLayerName;
        public GameObject[] goToBePooled;
        //public GameObject goToBePooled;
        public int nCount;
        public bool bExpandableList;
    }

    public class PoolingManager : Singleton<PoolingManager>
    {
        //===============================================================================================

        public List<PoolObject> listObjectsToPool;
        public List<GameObject> listPooledObjects;

        
        //===============================================================================================

        void Awake()
        {
            listPooledObjects = new List<GameObject>();
            foreach (PoolObject po in listObjectsToPool)
            {
                for (int i = 0; i < po.nCount; i++)
                {
                    if (po.goToBePooled != null)
                    {
                        GameObject go;
                        int nGroupSize = po.goToBePooled.Length;
                        for (int j = 0; j < nGroupSize; j++)
                        {
                            int nRandIndex = UnityEngine.Random.Range(0, nGroupSize);
                            go = Instantiate(po.goToBePooled[nRandIndex]);
                            if(go != null)
                            {
                                go.SetActive(false);
                                listPooledObjects.Add(go);
                            }
                        }

                        /*
                        GameObject go = Instantiate(po.goToBePooled);
                        if (go != null)
                        {
                            go.SetActive(false);
                            listPooledObjects.Add(go);
                        }
                        */
                    }
                }
            }
        }

        //===============================================================================================
        
        public GameObject GetPooledObject(string strLayer, string strTag = "")
        {
            for (int i = 0; i < listPooledObjects.Count; i++)
            {
                if (!listPooledObjects[i].activeInHierarchy && LayerMask.LayerToName(listPooledObjects[i].layer) == strLayer)
                {
                    if(!string.IsNullOrEmpty(strTag))
                    {
                        if(listPooledObjects[i].tag == strTag)
                        {
                            return listPooledObjects[i];
                        }
                    }
                    else
                    {
                        return listPooledObjects[i];
                    }
                }
            }

            foreach (PoolObject po in listObjectsToPool)
            {
                if (po.goToBePooled != null)
                {
                    if (po.strLayerName == strLayer)
                    {
                        if (po.bExpandableList)
                        {
                            Debug.Log("<color='red'>Expand List</color> -> " + strLayer);
                            int nGroupSize = po.goToBePooled.Length;

                            //@TODO: integrate tag support for expandable list
                            int nRandIndex = UnityEngine.Random.Range(0, nGroupSize);
                            GameObject go = Instantiate(po.goToBePooled[nRandIndex]);

                            go.SetActive(false);
                            listPooledObjects.Add(go);

                            return go;
                        }
                    }
                }
            }

            return null;
        }

        //===============================================================================================

        public void ReleasePooledObject(GameObject go, bool bScaleDown = false)
        {
            if (go != null)
            {
                go.transform.parent = null;
                go.SetActive(false);

                if (bScaleDown)
                {
                    go.transform.localScale = Vector3.zero;
                }

                //int nIndex = listPooledObjects.IndexOf(go);
                //listPooledObjects.RemoveAt(nIndex);
               // listPooledObjects.Add(go);
            }
        }

        //===============================================================================================

        public void ReleaseAllPooledObjects(string strLayer = null)
        {
            int nReleasedObjectCount = 0;

            for (int i = 0; i < listPooledObjects.Count; i++)
            {
                GameObject goCurrent = listPooledObjects[i];
                if (goCurrent == null)
                {
                    listPooledObjects.Remove(goCurrent);
                }
                else
                {
                    if (goCurrent.activeInHierarchy)
                    {
                        if (strLayer == null)
                        {
                            ReleasePooledObject(goCurrent);
                        }
                        else
                        {
                            if (LayerMask.LayerToName(goCurrent.layer) == strLayer)
                            {
                                nReleasedObjectCount++;
                                ReleasePooledObject(goCurrent);
                            }
                        }
                    }
                }
            }

            Debug.Log("<color='blue'>Released by layer </color> <color='white'>" + strLayer + "</color>" + "<color='green'> " + nReleasedObjectCount.ToString() + "</color>");
        }

        //===============================================================================================


    }
}
