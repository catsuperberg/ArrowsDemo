using System.Collections.Generic;
using UnityEngine;

public interface ITransformContainer
{
    public Transform MainTransform {get;}
    public List<Transform> ChildrenTransforms {get;}
}