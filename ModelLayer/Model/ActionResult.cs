﻿namespace ModelLayer.Model
{
    public class ActionResult<T>
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = " ";
        public T Data { get; set; } = default(T);
    }
}
