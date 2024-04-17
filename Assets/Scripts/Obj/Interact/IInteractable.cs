using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public void Interact();
}

public interface IBoolInteractable
{
    public void Interact(Interact interact);
}
