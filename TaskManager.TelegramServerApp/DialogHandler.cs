using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TaskManager.TelegramServerApp
{
    public class DialogHandler
    {
        private Dictionary<string, List<Func<Message, Task<bool>>>> flows =
            new Dictionary<string, List<Func<Message, Task<bool>>>>();

        private string lastFlowName;
        private int currentHandlerFunctionNumber;
        private string currentFlowName;

        public DialogHandler AddFlow(string flowName, Func<Message, Task<bool>> messageHandler)
        {
            if (string.IsNullOrEmpty(flowName))
            {
                return this;
            }

            if (messageHandler == null)
            {
                return this;
            }

            if (!flows.ContainsKey(flowName))
            {
                flows.Add(flowName, new List<Func<Message, Task<bool>>>()
                {
                    messageHandler
                });
            }

            lastFlowName = flowName;
            return this;
        }

        public DialogHandler AddMessageHandler(Func<Message, Task<bool>> messageHandler)
        {
            if (messageHandler == null)
            {
                return this;
            }

            if (string.IsNullOrEmpty(lastFlowName))
            {
                return this;
            }

            flows[lastFlowName].Add(messageHandler);

            return this;
        }

        public async Task Handle(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (string.IsNullOrEmpty(currentFlowName))
            {
                if (flows.ContainsKey(message.Text))
                {
                    currentFlowName = message.Text;
                    await flows[currentFlowName][currentHandlerFunctionNumber++].Invoke(message);
                }

                return;
            }

            if (currentHandlerFunctionNumber >= flows[currentFlowName].Count)
            {
                currentFlowName = null;
                currentHandlerFunctionNumber = 0;
                return;
            }

            if (!(await flows[currentFlowName][currentHandlerFunctionNumber++].Invoke(message)))
            {
                currentFlowName = null;
                currentHandlerFunctionNumber = 0;
                return;
            }
        }
    }
}
