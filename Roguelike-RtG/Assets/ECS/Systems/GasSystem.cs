namespace JS.ECS
{
    public class GasSystem : SystemBase<GasCloud>
    {
        private TimeSystem _timeSystem;

        private void Start()
        {
            _timeSystem = GetComponent<TimeSystem>();
            _timeSystem.onNewRound += OnNewRound;
        }

        private void OnDestroy()
        {
            _timeSystem.onNewRound -= OnNewRound;
            _timeSystem = null;
        }

        private void OnNewRound()
        {
            foreach(var cloud in components)
            {
                OnCloudApply(cloud);
            }
        }

        private void OnCloudApply(GasCloud cloud)
        {

        }

        private void DisperseCloud(GasCloud cloud)
        {
            if (!cloud.transform.currentNode.blocksGas)
            {

            }
            else
            {

            }
        }
    }
}

