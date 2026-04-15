using UnityEngine;

public class SpawnObjectByPropertisList : MonoBehaviour // Этот класс отвечает за создание объектов на основе списка свойств, заданных в массиве properties.
                                                        // Он использует префаб для создания объектов и устанавливает их родителем указанный Transform.
                                                        
{
    [SerializeField] private Transform parent;

    [Header("Prefabs")]
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private GameObject sliderPrefab; 

    [SerializeField] private ScriptableObject[] properties;

    [ContextMenu(nameof(SpawnInEditMode))]
    public void SpawnInEditMode()
    {
        if (Application.isPlaying) return;

        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(parent.GetChild(i).gameObject);
        }

        for (int i = 0; i < properties.Length; i++)
        {
            GameObject prefabToUse = buttonPrefab;

            if (properties[i] is AudioMixerFloatSetting)
            {
                prefabToUse = sliderPrefab;
            }

            GameObject go = Instantiate(prefabToUse, parent);

            IScriptableObjectProperty propertySetter = go.GetComponent<IScriptableObjectProperty>();
            if (propertySetter != null)
            {
                propertySetter.ApplyProperty(properties[i]);
            }
        }
    }
}
