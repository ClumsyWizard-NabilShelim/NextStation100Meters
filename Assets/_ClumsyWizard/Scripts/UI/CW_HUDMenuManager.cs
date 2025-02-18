using ClumsyWizard.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public interface IHUDMenu
{
    public bool Close();
}

public class CW_HUDMenuManager : CW_Persistant<CW_HUDMenuManager>, ISceneLoadEvent
{
    private List<IHUDMenu> menus = new List<IHUDMenu>();
    public bool IsAnyMenuOpen => menus.Count > 0;

    public void AddOpenMenu(IHUDMenu menu)
    {
        if (menus.Contains(menu))
            return;

        menus.Add(menu);
    }
    private void RemoveOpenMenu(IHUDMenu menu)
    {
        if (!menus.Contains(menu))
            return;

        menus.Remove(menu);
    }

    public void CloseLastMenu()
    {
        if (menus.Count == 0)
            return;

        if(menus[menus.Count - 1].Close())
            RemoveOpenMenu(menus[menus.Count - 1]);
    }

    //Clean Up
    public void OnSceneLoadOver(Action onComplete)
    {
        onComplete?.Invoke();
    }

    public void OnSceneLoadTriggered(Action onComplete)
    {
        for (int i = 0; i < menus.Count; i++)
        {
            menus[i].Close();
        }

        menus.Clear();

        onComplete?.Invoke();
    }
}