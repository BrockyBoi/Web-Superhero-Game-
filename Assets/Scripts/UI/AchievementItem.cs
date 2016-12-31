using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class AchievementItem : MonoBehaviour {
	public Text nameString;
	public Text amount; 
	public Image photo;

	string achievementName;
	string achievementAmount;

	public void GiveData(string nameString, string amountString)
	{
		this.nameString.text = nameString;
		amount.text = amountString;
	}

}
