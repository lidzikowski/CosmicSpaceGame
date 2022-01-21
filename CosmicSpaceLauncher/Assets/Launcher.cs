using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Launcher : MonoBehaviour
{
    public Button UpdateButton;
    public Text OutputText;
    public Text OutputProgressText;

    readonly string RepositoryURL = "https://api.github.com/repos/lidzikowski/CosmicSpace-Windows-Release/contents";
    readonly string AccessToken = "access_token=e2b4e4807026e43cb276c1c4868e14561e6cd8e5";

    List<FileClass> Catalogs;
    List<FileClass> Files;

    public List<RepositoryState> RepositoryDone = new List<RepositoryState>();
    public List<RepositoryState> RepositoryFileDone = new List<RepositoryState>();

    string GamePath => PlayerPrefs.GetString("CosmicSpaceGamePath");
    string GameExe => PlayerPrefs.GetString("CosmicSpaceGamePathExe");

    string FileSizeFormat(long bytes)
    {
        return (bytes / (double)Mathf.Pow(1024, 2)).ToString("0.00") + " Mb";
    }

    void Start()
    {
        UpdateButton?.onClick.AddListener(UpdateButton_Click);

        Message("Hello world ( ͡° ͜ʖ ͡°)");
    }

    void UpdateButton_Click()
    {
        StartCoroutine(GetRepository());


    }

    IEnumerator GetRepository()
    {
        Catalogs = new List<FileClass>();
        Files = new List<FileClass>();

        RepositoryDone = new List<RepositoryState>();
        RepositoryFileDone = new List<RepositoryState>();

        StartCoroutine(GetContent(RepositoryURL + "?" + AccessToken));
        yield return new WaitUntil(() => RepositoryDone.All(o => o.Ready));

        Message($"Z repozytorium pobrano dane:");
        Message($"Katalogi: {Catalogs.Count}");
        Message($"Pliki: {Files.Count} [Razem: {FileSizeFormat(Files.Sum(o => o.size))}]");

        StartCoroutine(CheckGameFiles());
        yield return new WaitUntil(() => RepositoryFileDone.All(o => o.Ready));

        Message($"Lokalne pliki gry zostaly uaktualnione:");
        Message($"Pobrano: {RepositoryFileDone.Count} [Razem: {FileSizeFormat(RepositoryFileDone.Sum(o => o.File.size))}]");

        Message($"Wlaczanie klienta gry...");

        if(!File.Exists(GameExe))
        {
            string path = Path.Combine(GamePath, "CosmicSpace.exe");
            if (File.Exists(path))
                PlayerPrefs.SetString("CosmicSpaceGamePathExe", path);
        }

        Message($"Run: {GameExe}");
        try
        {
            System.Diagnostics.Process.Start(GameExe);
            Application.Quit();
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    IEnumerator GetContent(string url)
    {
        RepositoryState state = new RepositoryState();
        RepositoryDone.Add(state);

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                FileDecoder files = null;
                try
                {
                    files = JsonUtility.FromJson<FileDecoder>("{\"result\":" + request.downloadHandler.text + "}");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(ex.Message);
                }

                if (files != null && files.result.Length > 0)
                {
                    foreach (FileClass file in files.result)
                    {
                        if (file.type.Equals("dir"))
                        {
                            //Message($"[{file.type}] {file.path}");
                            Catalogs.Add(file);

                            StartCoroutine(GetContent(file.url + "&" + AccessToken));
                        }
                        else
                        {
                            if (!file.name.Equals(".gitattributes") && !file.name.Equals(".gitignore"))
                            {
                                //Message($"[{file.type}] {file.path}");

                                if (file.name.Contains(".resS"))
                                {
                                    RepositoryState resourcesState = new RepositoryState();
                                    StartCoroutine(GetLargeFile(file, resourcesState));
                                    while (!resourcesState.Ready)
                                        yield return new WaitForEndOfFrame();
                                }
                                Files.Add(file);
                            }
                        }
                    }
                }
            }
        }

        state.Ready = true;
    }

    IEnumerator GetLargeFile(FileClass file, RepositoryState state)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(file._links.self + "&" + AccessToken))
        {
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                FileClass fileClass = null;
                try
                {
                    fileClass = JsonUtility.FromJson<FileClass>(request.downloadHandler.text);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(ex.Message);
                }

                if (fileClass != null)
                {
                    file.size = fileClass.size;
                    file.download_url = fileClass.download_url;
                }
            }
        }

        state.Ready = true;
    }

    IEnumerator CheckGameFiles()
    {
        string localPath = !string.IsNullOrEmpty(GamePath) ? GamePath : Path.Combine(Application.dataPath, "CosmicSpaceGame");

        PlayerPrefs.SetString("CosmicSpaceGamePath", localPath);
        Message($"Ustawiona sciezka z plikami gry: {localPath}");

        // Sprawdzenie katalogow
        foreach (FileClass catalog in Catalogs)
        {
            string dirPath = Path.Combine(localPath, catalog.path);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
        }

        // Sprawdzenie plikow
        foreach (FileClass file in Files)
        {
            string filePath = Path.Combine(localPath, file.path);
            if (!File.Exists(filePath))
            {
                StartCoroutine(DownloadFile(file, filePath));
            }
            else
            {
                FileInfo fileInfo = new FileInfo(filePath);
                if(!fileInfo.Length.Equals(file.size))
                {
                    File.Delete(filePath);
                    StartCoroutine(DownloadFile(file, filePath));
                }
            }
        }

        yield return 0;
    }

    IEnumerator DownloadFile(FileClass file, string savePath)
    {
        RepositoryState state = new RepositoryState()
        {
            File = file
        };
        RepositoryFileDone.Add(state);

        using (UnityWebRequest www = UnityWebRequest.Get(file.download_url))
        {
            StartCoroutine(DownloadProgress(www, state));
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                if (file.name.Equals("CosmicSpace.exe"))
                    PlayerPrefs.SetString("CosmicSpaceGamePathExe", savePath);

                File.WriteAllBytes(savePath, www.downloadHandler.data);
            }
        }

        state.Ready = true;
    }

    IEnumerator DownloadProgress(UnityWebRequest www, RepositoryState fileState)
    {
        while (!www.isDone)
        {
            fileState.Progress = www.downloadProgress;
            RefreshDownloadProgress();
            yield return new WaitForEndOfFrame();
        }
    }

    void Message(string message)
    {
        if (OutputText == null)
            return;

        OutputText.text += $"> {message}\n";
    }

    void RefreshDownloadProgress()
    {
        if (OutputProgressText == null)
            return;

        OutputProgressText.text = string.Empty;
        foreach (RepositoryState state in RepositoryFileDone.Where(o => !o.Ready))
        {
            string status = $"Downloading [{(string.Format("{0:P1}", state.Progress))}]";
            OutputProgressText.text += $"> [{status}] {state.File.name} ({FileSizeFormat(state.File.size)})";

            OutputProgressText.text += "\n";
        }
    }
}