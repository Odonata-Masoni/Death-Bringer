﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class CharacterEvents
{
    public static UnityAction<GameObject, float> characterDamaged;
    public static UnityAction<GameObject, float> characterHealed; 
}