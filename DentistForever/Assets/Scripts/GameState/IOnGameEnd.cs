using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public interface IOnGameEnd
{
    public UniTask OnGameEnd(CancellationToken cancellationToken);
}
