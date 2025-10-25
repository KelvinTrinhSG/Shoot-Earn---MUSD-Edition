using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

namespace GunslingerGame
{
	/// <summary>
	/// Selects a certain button when this canvas/panel/object is enabled
	/// </summary>
	public class GSGSelectButton:MonoBehaviour 
	{
		// The button to select
		public GameObject selectedButton;

		/// <summary>
		/// Runs when this object is enabled
		/// </summary>
		void OnEnable() 
		{
			if ( selectedButton )    
			{
				// Select the button
				EventSystem.current.SetSelectedGameObject(selectedButton);
			}	
		}
	}
}
