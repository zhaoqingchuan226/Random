using UnityEngine;

namespace RadiantGI.Universal {

    [ExecuteInEditMode]
    public class RadiantVirtualEmitter : MonoBehaviour {

        [Header("GI Color")]
        [ColorUsage(showAlpha: false, hdr: true)]
        public Color color = new Color(1, 1, 1);
        [Tooltip("Enable this option to add the emission color of the material used by this object to the global illumination.")]
        public bool addMaterialEmission;
        [Tooltip("The renderer from which synchronize the emission color")]
        public Renderer targetRenderer;
        public string emissionPropertyName = "_EmissionColor";
        [Tooltip("Useful in case the gameobject uses more than one material")]
        public int materialIndex;
        public float intensity = 1f;

        [Header("Area Of Influence")]
        public Vector3 boxCenter;
        public Vector3 boxSize = new Vector3(25, 25, 25);

        int emissionNameId;
        Material mat;
        Renderer thisRenderer;

        private void OnValidate() {
            intensity = Mathf.Max(0, intensity);
        }

        void OnEnable() {
            emissionNameId = Shader.PropertyToID(emissionPropertyName);
            thisRenderer = GetComponentInChildren<Renderer>();
            RadiantRenderFeature.RegisterVirtualEmitter(this);
        }

        void OnDisable() {
            RadiantRenderFeature.UnregisterVirtualEmitter(this);
        }

        void OnDrawGizmosSelected() {
            Gizmos.color = new Color(color.r, color.g, color.b, 0.75F);
            Gizmos.DrawWireCube(transform.position + boxCenter, boxSize);
        }


        public Color GetGIColor() {
            Color sum = color;
            if (addMaterialEmission) {
                Renderer r = targetRenderer != null ? targetRenderer : thisRenderer;
                if (r != null) {
                    if (materialIndex > 0 && materialIndex < r.sharedMaterials.Length) {
                        mat = r.sharedMaterials[materialIndex];
                    } else {
                        mat = r.sharedMaterial;
                    }
                    if (mat != null && mat.HasProperty(emissionNameId)) {
                        sum += mat.GetColor(emissionNameId);
                    }
                }
            }
            return sum * intensity;
        }


    }

}
