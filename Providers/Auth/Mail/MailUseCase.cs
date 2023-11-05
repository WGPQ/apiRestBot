using ApiRestBot.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRestBot.Providers.Auth.Mail
{
    
    public interface MailUseCase<T> where T : new()
    {
        bool SendEmail(T entiti);
    }

    public interface IMailService : MailUseCase<MailEntity> { }


}
