using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemController : MonoBehaviour
{
    // �g�p�ł���A�C�e���̍ő吔
    public int maxItemNumber { get; private set; }

    // �A�C�e���̎c��̐�
    public int itemNumber { get; private set; }

    // �����\�����邽�߂̃e�L�X�g
    [SerializeField]
    private TextMeshProUGUI numberText;

    // �A�C�e���̃A�C�R��
    [SerializeField]
    private Image itemImage;

    // Start is called before the first frame update
    void Start()
    {
        // �A�C�e���̐���ݒ�
        maxItemNumber = 5;
        itemNumber = maxItemNumber;
    }

    private void FixedUpdate()
    {
        ItemNumberDisplay();

        if(itemNumber == 0)
        {
            ItemColorChange();
        }
    }

    /// <summary>
    /// �c��A�C�e��������ʏ�ɕ\��������
    /// </summary>
    private void ItemNumberDisplay()
    {
        // �c��A�C�e������\��������
        numberText.GetComponent<TextMeshProUGUI>().text = "<sprite=" + itemNumber + ">";
    }

    /// <summary>
    /// �A�C�e���A�C�R���̐F��ύX������
    /// </summary>
    private void ItemColorChange()
    {
        // �A�C�e���A�C�R���̐F��ύX����
        itemImage.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
    }

    /// <summary>
    /// �c��A�C�e�����Đݒ�p�̊֐�
    /// </summary>
    /// <param name="num">
    /// �A�C�e���̐�
    /// </param>
    public void SetItemNumber(int num)
    {
        itemNumber = num;
    }
}
