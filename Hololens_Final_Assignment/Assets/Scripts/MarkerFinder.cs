using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MarkerFinder : MonoBehaviour
{
    Microsoft.MixedReality.QR.QRCodeWatcher watcher = null;


    Dictionary<string, QRMarker> MarkerDB;
    List<Tuple<Guid, float, string>> Schedule;
    public QRMarker MarkerPrefab;
    
    async void Start()
    {
//#if !UNITY_EDITOR
        var task = Microsoft.MixedReality.QR.QRCodeWatcher.RequestAccessAsync();
        while( await Task.WhenAny(task, Task.Delay(3000)) != task){ }
        if (task.Result != Microsoft.MixedReality.QR.QRCodeWatcherAccessStatus.Allowed)
        {
            UnityEngine.WSA.Application.InvokeOnUIThread(async () =>
            {

               // Windows.UI.Popups.MessageDialog dialogue = new Windows.UI.Popups.MessageDialog("설정으로 이동하여서 카메라 권한을 활성화해주세요", "권한 없음");
               // dialogue.Commands.Add(new Windows.UI.Popups.UICommand("확인", (command) => Application.Quit()));
                //await dialogue.ShowAsync();
            }, false);
        }
        else 
//#endif
       
        
        MarkerFindProcess();
    }
    void MarkerFindProcess()
    {
        Schedule = new List<Tuple<Guid, float, string>>();
        MarkerDB = new Dictionary<string, QRMarker>();
        watcher = new Microsoft.MixedReality.QR.QRCodeWatcher();
        watcher.Added += (sender, args) =>
        {
            Schedule.Add(Tuple.Create(args.Code.SpatialGraphNodeId, args.Code.PhysicalSideLength, args.Code.Data));
        };
        watcher.Added += (sender, args) =>
        {
            MarkerDB[args.Code.Data].ID = args.Code.SpatialGraphNodeId;
            MarkerDB[args.Code.Data].Size = args.Code.PhysicalSideLength;
        };
        watcher.Added += (sender, args) =>
        {
            var detectedCode = args.Code;
            var markerID = detectedCode.SpatialGraphNodeId;
            var markerSize = detectedCode.PhysicalSideLength;
            var markerData = detectedCode.Data;
        };
        watcher.Start();
    }
    private void Update()
    {
        while(Schedule.Count != 0)
        {
            var item = Schedule[0];
            var marker = Instantiate(MarkerPrefab.gameObject).GetComponent<QRMarker>();
            marker.ID = item.Item1;
            marker.Size = item.Item2;
            MarkerDB[item.Item3] = marker;
            Schedule.Remove(item);
            
        }
    }
    private void OnDestroy()
    {
        watcher.Stop();
    }
}
