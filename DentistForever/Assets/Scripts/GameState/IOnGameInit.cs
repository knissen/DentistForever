using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public interface IOnGameInit
{
    public UniTask IOnGameInit(CancellationToken cancellationToken);
}
