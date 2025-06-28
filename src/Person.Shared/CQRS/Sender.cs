using Microsoft.Extensions.DependencyInjection;


namespace Person.Shared.CQRS
{
    public class Sender: ISender
    {


        private readonly IServiceProvider _serviceProvider;
        public Sender(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            // Find the specific handler
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
            var handler = _serviceProvider.GetService(handlerType);
            if (handler == null)
            {
                throw new InvalidOperationException($"Send- Handler not found for request type {request.GetType().Name}");
            }

            // Call the Handle method
            var method = handlerType.GetMethod("Handle");
            if (method == null)
            {
                throw new InvalidOperationException($"Send- Handle method not found for request type {request.GetType().Name}");
            }
            return (Task<TResponse>)method.Invoke(handler, new object[] { request, cancellationToken });
        }



        public Task<TResponse> SendFaster<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            try
            {
                // Find the specific handler
                var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
                dynamic handler = _serviceProvider.GetRequiredService(handlerType);

                // Directly call the Handle method using dynamic
                return handler.Handle((dynamic)request, cancellationToken);
            }
            catch (InvalidOperationException ex)
            {
                // Handle cases where the handler is not found or other DI-related issues
                throw new InvalidOperationException($"SendFaster Failed to resolve handler for request type {request.GetType().Name}.", ex);
            }
            catch (Exception ex)
            {
                // Log or handle unexpected exceptions
                throw new Exception($"SendFaster- An error occurred while processing the request of type {request.GetType().Name}.", ex);
            }
        }

    }

}
