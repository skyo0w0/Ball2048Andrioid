using Cinemachine;
using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    [SerializeField] private GameObject _levelGen;
    [SerializeField] private GameObject _playerBallPrefab;
    [SerializeField] private CinemachineVirtualCamera _cinemachineCameraPrefab;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private LevelManager _levelManagerPrefab;
    [SerializeField] private AudioManager _audioManagerPrefab;

    public override void InstallBindings()
    {
        Container.Bind<PointerInputManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();

        var audioManager = Container.InstantiatePrefabForComponent<AudioManager>(_audioManagerPrefab);
        Container.Bind<AudioManager>().FromInstance(audioManager).AsSingle();

        var levelGenerator = Container.InstantiatePrefabForComponent<LevelGenerator>(_levelGen);
        Container.Bind<LevelGenerator>().FromInstance(levelGenerator).AsSingle();

        Container.Bind<LevelManager>().FromComponentInNewPrefab(_levelManagerPrefab).AsSingle().NonLazy();

        var cinemachineCamera = Container.InstantiatePrefabForComponent<CinemachineVirtualCamera>(_cinemachineCameraPrefab);
        Container.Bind<CinemachineVirtualCamera>().FromInstance(cinemachineCamera).AsSingle();

        var playerBall = Container.InstantiatePrefabForComponent<PlayerBall>(_playerBallPrefab);
        Container.Bind<PlayerBall>().FromInstance(playerBall).AsSingle();

    }

}