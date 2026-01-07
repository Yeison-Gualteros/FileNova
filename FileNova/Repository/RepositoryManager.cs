using Contracts;
using Contracts.Interface;
using Repository.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly RepositoryContext _repositoryContext;
        private readonly Lazy<IDocumentoRepository> _documentoRepository;
        private readonly Lazy<ISolicitudRepository> _solicitudRepository;
        private readonly Lazy<ITrazabilidad_DocumentoRepository> _trazabilidad_DocumentoRepository;
        private readonly Lazy<IRoleRepository> _roleRepository;

        public RepositoryManager(RepositoryContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
            _documentoRepository = new Lazy<IDocumentoRepository>(() => new DocumentoRepository(repositoryContext));
            _solicitudRepository = new Lazy<ISolicitudRepository>(() => new SolicitudRepository(repositoryContext));
            _trazabilidad_DocumentoRepository = new Lazy<ITrazabilidad_DocumentoRepository>(() => new Trazabilidad_DocumentoRepository(repositoryContext));
            _roleRepository = new Lazy<IRoleRepository>(() => new RoleRepository(repositoryContext));
        }

        public IDocumentoRepository Documento => _documentoRepository.Value;
        public ISolicitudRepository Solicitud => _solicitudRepository.Value;
        public ITrazabilidad_DocumentoRepository trazabilidad_Documento => _trazabilidad_DocumentoRepository.Value;
        public IRoleRepository Role => _roleRepository.Value;


        public async Task SaveAsync()
        {
            await _repositoryContext.SaveChangesAsync();
        }
    }
}
