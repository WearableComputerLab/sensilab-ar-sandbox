//  
//  UI_FireSimulationMenu.cs
//
//	Copyright 2021 SensiLab, Monash University <sensilab@monash.edu>
//
//  This file is part of sensilab-ar-sandbox.
//
//  sensilab-ar-sandbox is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  sensilab-ar-sandbox is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with sensilab-ar-sandbox.  If not, see <https://www.gnu.org/licenses/>.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ARSandbox.FireSimulation
{
    public class UI_FireSimulationMenu : MonoBehaviour
    {
        public FireSimulation FireSimulation;
        public LineDrawingManager LineDrawingManager;
        public GameObject UI_FireOptionMenu;
        public GameObject UI_FireBreakMenu;
        public UI_Dial UI_WindDirectionDial;
        public Slider UI_WindSpeedSlider;
        public Text UI_PlayPauseBtnText;
        public Slider UI_ZoomSlider;
        public Slider UI_FireBreakThicknessSlider;
        public GameObject UI_FireOptionMenuBtn;
        public GameObject UI_FireBreakMenuBtn;

        public void OpenMenu()
        {
            UI_FireOptionMenu.SetActive(true);
            UI_FireBreakMenu.SetActive(false);
            UI_WindDirectionDial.SetDialRotation(FireSimulation.WindDirection, false);
            UI_WindSpeedSlider.value = FireSimulation.WindSpeed;
            UI_ZoomSlider.value = FireSimulation.LandscapeZoom;
            UI_FireBreakThicknessSlider.value = LineDrawingManager.FireBreakThickness;

            if (FireSimulation.SimulationPaused)
            {
                UI_PlayPauseBtnText.text = "Resume Simulation";
            }
            else
            {
                UI_PlayPauseBtnText.text = "Pause Simulation";
            }
        }

        public void UI_TogglePauseSimulation()
        {
            bool simulationPaused = FireSimulation.TogglePauseSimulation();
            if (simulationPaused)
            {
                UI_PlayPauseBtnText.text = "Resume Simulation";
            } else
            {
                UI_PlayPauseBtnText.text = "Pause Simulation";
            }
        }

        public void UI_OpenFireBreakMenu()
        {
            UI_FireOptionMenu.SetActive(false);
            UI_FireBreakMenu.SetActive(true);
        }
    }
}
