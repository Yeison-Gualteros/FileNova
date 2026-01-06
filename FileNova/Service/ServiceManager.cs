using AutoMapper;
using Contracts;
using Contracts.Interface;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Service.Contracts;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public sealed class ServiceManager : IServiceManager
    {
        private readonly Lazy<IDocumentoService> _documentoService;
        private readonly Lazy<ISolicitudService> _solicitudService;
        private readonly Lazy<ITrazabilidad_DocumentoService> _trazabilidad_DocumentoService;
        private readonly Lazy<IAuthenticationService> _authenticationService;
        public ServiceManager(IRepositoryManager repositoryManager, ILoggerManager logger, IMapper mapper, UserManager<User> userManager, IOptions<JwtConfiguration> configuration)
        {
            _documentoService = new Lazy<IDocumentoService>(() => new DocumentoService(repositoryManager, logger, mapper));
            _solicitudService = new Lazy<ISolicitudService>(() => new SolicitudService(repositoryManager, logger, mapper));
            _trazabilidad_DocumentoService = new Lazy<ITrazabilidad_DocumentoService>(() => new Trazabilidad_DocumentoService(repositoryManager, logger, mapper));
            _authenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(logger, mapper, userManager, configuration));

        }
        public IDocumentoService DocumentoService => _documentoService.Value;
        public ISolicitudService SolicitudService => _solicitudService.Value;
        public ITrazabilidad_DocumentoService Trazabilidad_DocumentoService => _trazabilidad_DocumentoService.Value;
        public IAuthenticationService AuthenticationService => _authenticationService.Value;

    }
}
