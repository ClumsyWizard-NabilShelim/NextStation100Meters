using System.Collections;
using TMPro;
using UnityEngine;
public class ToolTip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private TextMeshProUGUI description;

    private void Start()
    {
        Show(false);
    }

    public void SetInfo(string labelText, string descriptionText)
    {
        label.text = labelText;
        description.text = descriptionText;
    }

    public void Show(bool show)
    {
        transform.GetChild(0).gameObject.SetActive(show);
    }
}