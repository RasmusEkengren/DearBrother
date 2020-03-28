﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TarManSFXTrigger : MonoBehaviour
{
	public TarManSoundManager tarManSoundManager;
    public void Walk()
	{
		if (tarManSoundManager != null)
		{
			tarManSoundManager.TarManWalkSound();
		}
	}
}
