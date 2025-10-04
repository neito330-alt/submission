using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using UnityEngine.Rendering.HighDefinition;
using Unity.VisualScripting;


namespace Rooms.PanelSystem
{
    [Serializable]
    public class DecalData
    {
        public Texture2D image;

        public Color color;
        public float emission;

        public float scale;
        public Vector2 position;

        [HideInInspector]
        public bool canDelete = false;

        [HideInInspector]
        public DecalProjector projector;
    }

    [Serializable]
    public class DecalItem
    {
        public Vector2 position;
        public float scale = 1f;
        public bool canDelete = false;
        public Material material;
        public DecalProjector projector;
    }


    public class DecalManager : MonoBehaviour
    {
        public List<DecalItem> decals = new List<DecalItem>();

        [SerializeField]
        private float _rotation = 0;
        public float Rotation
        {
            get => _rotation;
            set
            {
                _rotation = value;
                foreach (var decal in decals)
                {
                    if (decal.projector != null)
                    {
                        decal.projector.transform.localEulerAngles = new Vector3(0, 0, _rotation);
                    }
                }
            }
        }

        public GameObject decalPrefab;
        public Material materialCash;

        public DecalProjector AddDecal(DecalItem decal)
        {
            decal.projector = Instantiate(decalPrefab, transform).GetComponent<DecalProjector>();
            decal.projector.material.enableInstancing = true;

            decal.projector.material = decal.material;

            decal.projector.transform.localPosition = new Vector3(decal.position.x, decal.position.y, 0);
            decal.projector.transform.localScale = new Vector3(decal.scale, decal.scale, 1);
            decal.projector.transform.localEulerAngles = new Vector3(0, 0, Rotation);

            decals.Add(decal);

            return decal.projector;
        }

        public DecalProjector AddDecal(DecalData decal)
        {
            decal.projector = Instantiate(decalPrefab, transform).GetComponent<DecalProjector>();
            decal.projector.material.enableInstancing = true;

            Material material = new Material(decal.projector.material);

            material.SetTexture("_MainTex", decal.image);
            material.SetColor("_Color", decal.color);
            material.SetFloat("_Emission", decal.emission);

            decal.projector.material = material;

            decal.projector.transform.localPosition = new Vector3(decal.position.x, decal.position.y, 0);
            decal.projector.transform.localScale = new Vector3(decal.scale, decal.scale, 1);
            decal.projector.transform.localEulerAngles = new Vector3(0, 0, Rotation);

            DecalItem decalItem = new DecalItem
            {
                position = decal.position,
                scale = decal.scale,
                material = material,
                projector = decal.projector,
                canDelete = decal.canDelete
            };

            decals.Add(decalItem);

            return decal.projector;
        }

        public void RemoveDecal(DecalProjector projector)
        {
            if (projector != null)
            {
                if (projector.material != null)
                {
                    Destroy(projector.material);
                }
                Destroy(projector.gameObject);
                decals.RemoveAll(d => d.projector == projector);
            }
        }

        public void RemoveDecal(Vector2 position)
        {
            List<DecalItem> decalsToRemove = new List<DecalItem>(decals);
            foreach (var decal in decalsToRemove)
            {
                if (Vector2.Distance(decal.position, position) <= decal.scale / 2 && decal.canDelete)
                {
                    RemoveDecal(decal.projector);
                }
            }
        }

        public void ClearDecals()
        {
            foreach (var decal in decals)
            {
                if (decal.projector != null)
                {
                    if (Application.isPlaying)
                    {
                        Destroy(decal.projector.gameObject);
                    }
                    else
                    {
                        DestroyImmediate(decal.projector.gameObject);
                    }
                }
            }
            decals.Clear();
        }

        public void UpdateDecal()
        {
            List<DecalItem> tmp = new List<DecalItem>(decals);
            foreach (var decal in tmp)
            {
                if (decal.projector.material == null)
                {
                    Destroy(decal.projector.gameObject);
                    decals.Remove(decal);
                }
            }
        }

        void OnDestroy()
        {
            //Destroy(materialCash);
        }
    }
}
