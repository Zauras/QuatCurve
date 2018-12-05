using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
/*
namespace Master
{

  [AlwaysUpdateSystem]
  public class HUDSystem : ComponentSystem
  {

      public struct PlayerData
      {
          public readonly int Length;
          [ReadOnly] public EntityArray Entity;
          [ReadOnly] public ComponentDataArray<PlayerInput> Input;
      }


//[Inject] PlayerData m_Players;

public Button NewGameButton;
        //public Text HealthText;

        protected override void OnStartRunning()
        {
            NewGameButton = GameObject.Find("StepButton").GetComponent<Button>();
            //HealthText = GameObject.Find("HealthText").GetComponent<Text>();

            NewGameButton.onClick.AddListener(MovementControllerSystem.MovementStep);

            //NewGameButton.gameObject.SetActive(true);
            Debug.Log("SetupButton");
        }

        protected override void OnUpdate()
        {

        }
 

    }

}


        /*
        private void UpdateDead()
        {
            if (HealthText != null)
            {
                HealthText.gameObject.SetActive(false);
            }
            if (NewGameButton != null)
            {
                NewGameButton.gameObject.SetActive(true);
            }
        }

        private void UpdateAlive()
        {
            HealthText.gameObject.SetActive(true);
            NewGameButton.gameObject.SetActive(false);

            int displayedHealth = (int)m_Players.Health[0].Value;

            if (m_CachedHealth != displayedHealth)
            {
                HealthText.text = $"HEALTH: {displayedHealth}";
                m_CachedHealth = displayedHealth;
            }
        }
        */

