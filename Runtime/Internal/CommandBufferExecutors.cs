using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace DCFApixels.DebugXCore.Internal
{
    internal interface ICommandBufferExecutor
    {
        void Execute(CommandBuffer cb);
    }
    internal class CommandBufferExecutorSRP : ICommandBufferExecutor
    {
        [ThreadStatic]
        private static CommandBufferExecutorSRP _instance = new CommandBufferExecutorSRP();
        public static CommandBufferExecutorSRP GetInstance(ScriptableRenderContext context)
        {
            if (_instance == null) { _instance = new CommandBufferExecutorSRP(); }
            _instance.RenderContext = context;
            return _instance;
        }
        public ScriptableRenderContext RenderContext;
        private CommandBufferExecutorSRP() { }
        public void Execute(CommandBuffer cb)
        {
            RenderContext.ExecuteCommandBuffer(cb);
        }
    }
    internal class CommandBufferExecutorBRP : ICommandBufferExecutor
    {
        [ThreadStatic]
        private static CommandBufferExecutorBRP _instance = new CommandBufferExecutorBRP();
        public static CommandBufferExecutorBRP GetInstance()
        {
            if (_instance == null) { _instance = new CommandBufferExecutorBRP(); }
            return _instance;
        }
        private CommandBufferExecutorBRP() { }
        public void Execute(CommandBuffer cb)
        {
            Graphics.ExecuteCommandBuffer(cb);
        }
    }
}