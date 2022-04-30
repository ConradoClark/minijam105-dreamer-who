using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameVariablesUpdater : MonoBehaviour
{
    private readonly Dictionary<Type, Action> _clearActions = new Dictionary<Type, Action>();

    private void Update()
    {
        Clear();
    }

    private void OnDisable()
    {
        Clear();
    }

    private void Clear()
    {
        foreach (var action in _clearActions.Values)
        {
            action();
        }
        _clearActions.Clear(); ;
    }

    public T Get<T>(FrameVariableDefinition<T> definition)
    {
        var (result, clearAction) = FrameVariables<T>.Get(definition.Variable, definition.Getter);
        _clearActions[typeof(T)] = clearAction;
        return result;
    }
}
