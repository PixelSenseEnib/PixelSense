using UnityEngine;
using System.Collections;

public class slideControl : MonoBehaviour {

	public GameObject slide1;
	public GameObject slide2;
	public GameObject slide3;
	public GameObject slide4;
	public GameObject slide5;
	public GameObject slide6;
	public GameObject slide7;

	private int nb = 0;
	private bool nbCanged = true;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Keypad4))
		{
			slide1.SetActive(false);
			nb--;
		}
		else if (Input.GetKeyDown(KeyCode.Keypad6))
		{
			slide1.SetActive(true);
			nb++;
		}

		if( nbCanged )
		{
			slide1.SetActive(false);
			slide2.SetActive(false);
			slide3.SetActive(false);
			slide4.SetActive(false);
			slide5.SetActive(false);
			slide6.SetActive(false);
			slide7.SetActive(false);

			switch(nb){
			case 0:
				slide1.SetActive(true);
				break;
			case 1:
				slide2.SetActive(true);
				break;
			case 2:
				slide3.SetActive(true);
				break;
			case 3:
				slide4.SetActive(true);
				break;
			case 4:
				slide5.SetActive(true);
				break;
			case 5:
				slide6.SetActive(true);
				break;
			case 6:
				slide7.SetActive(true);
				break;
			}
		}
	}
}
