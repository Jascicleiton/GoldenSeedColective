using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class UIInventoryTextBox : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textTop1, textTop2, textTop3;
    [SerializeField] private TextMeshProUGUI textBottom1, textBottom2, textBottom3;

    // Set text values
    public void SetTextBoxText(string textTop1, string textTop2, string textTop3, string textBottom1, string textBottom2, string textBottom3)
    {
        this.textTop1.text = textTop1;
        this.textTop2.text = textTop2;
        this.textTop3.text = textTop3;
        this.textBottom1.text = textBottom1;
        this.textBottom2.text = textBottom2;
        this.textBottom3.text = textBottom3;
    }
}
