namespace vehicle_rental.application.Common.Errors;

public static class ErrorCodes
{
    // Delivery Person Errors
    public const string DELIVERY_PERSON_NOT_FOUND = "DELIVERY_PERSON_NOT_FOUND";
    public const string DELIVERY_PERSON_CNPJ_EXISTS = "DELIVERY_PERSON_CNPJ_EXISTS";
    public const string DELIVERY_PERSON_LICENSE_EXISTS = "DELIVERY_PERSON_LICENSE_EXISTS";
    public const string DELIVERY_PERSON_CANNOT_RENT = "DELIVERY_PERSON_CANNOT_RENT";
    public const string DELIVERY_PERSON_INVALID_LICENSE_TYPE = "DELIVERY_PERSON_INVALID_LICENSE_TYPE";

    // Motorcycle Errors
    public const string MOTORCYCLE_NOT_FOUND = "MOTORCYCLE_NOT_FOUND";
    public const string MOTORCYCLE_LICENSE_PLATE_EXISTS = "MOTORCYCLE_LICENSE_PLATE_EXISTS";
    public const string MOTORCYCLE_HAS_ACTIVE_RENTALS = "MOTORCYCLE_HAS_ACTIVE_RENTALS";
    public const string MOTORCYCLE_NOT_AVAILABLE = "MOTORCYCLE_NOT_AVAILABLE";

    // Rental Errors
    public const string RENTAL_NOT_FOUND = "RENTAL_NOT_FOUND";
    public const string RENTAL_ALREADY_COMPLETED = "RENTAL_ALREADY_COMPLETED";
    public const string RENTAL_INVALID_PLAN = "RENTAL_INVALID_PLAN";
    public const string RENTAL_PERIOD_CONFLICT = "RENTAL_PERIOD_CONFLICT";
    public const string RENTAL_MAPPING_FAILED = "RENTAL_MAPPING_FAILED";

    // Auth Errors
    public const string AUTH_INVALID_CREDENTIALS = "AUTH_INVALID_CREDENTIALS";
    public const string AUTH_USER_NOT_FOUND = "AUTH_USER_NOT_FOUND";
    public const string AUTH_USER_ALREADY_EXISTS = "AUTH_USER_ALREADY_EXISTS";
    public const string AUTH_INSUFFICIENT_PERMISSIONS = "AUTH_INSUFFICIENT_PERMISSIONS";
    public const string AUTH_VALIDATION_ERROR = "AUTH_VALIDATION_ERROR";
    public const string AUTH_USER_BLOCKED = "AUTH_USER_BLOCKED";
    public const string AUTH_USER_LOCKED_OUT = "AUTH_USER_LOCKED_OUT";

    // File Errors
    public const string FILE_INVALID_FORMAT = "FILE_INVALID_FORMAT";
    public const string FILE_TOO_LARGE = "FILE_TOO_LARGE";
    public const string FILE_UPLOAD_FAILED = "FILE_UPLOAD_FAILED";
}

public static class ErrorMessages
{
    // Delivery Person Messages
    public const string DELIVERY_PERSON_NOT_FOUND = "Entregador não encontrado";
    public const string DELIVERY_PERSON_CNPJ_EXISTS = "Já existe um entregador cadastrado com este CNPJ";
    public const string DELIVERY_PERSON_LICENSE_EXISTS = "Já existe um entregador cadastrado com este número de CNH";
    public const string DELIVERY_PERSON_CANNOT_RENT = "Entregador não pode alugar motocicletas com o tipo de CNH atual";
    public const string DELIVERY_PERSON_INVALID_LICENSE_TYPE = "Tipo de CNH inválido para aluguel de motocicletas";

    // Motorcycle Messages
    public const string MOTORCYCLE_NOT_FOUND = "Motocicleta não encontrada";
    public const string MOTORCYCLE_LICENSE_PLATE_EXISTS = "Já existe uma motocicleta cadastrada com esta placa";
    public const string MOTORCYCLE_HAS_ACTIVE_RENTALS = "Não é possível excluir motocicleta com locações ativas";
    public const string MOTORCYCLE_NOT_AVAILABLE = "Motocicleta não está disponível para o período solicitado";

    // Rental Messages
    public const string RENTAL_NOT_FOUND = "Locação não encontrada";
    public const string RENTAL_ALREADY_COMPLETED = "Locação já foi finalizada";
    public const string RENTAL_INVALID_PLAN = "Plano de locação inválido";
    public const string RENTAL_PERIOD_CONFLICT = "Conflito de período com outras locações";
    public const string RENTAL_MAPPING_FAILED = "Falha ao converter dados da locação";

    // Auth Messages
    public const string AUTH_INVALID_CREDENTIALS = "Email ou senha inválidos";
    public const string AUTH_USER_NOT_FOUND = "Usuário não encontrado";
    public const string AUTH_USER_ALREADY_EXISTS = "Usuário já existe";
    public const string AUTH_INSUFFICIENT_PERMISSIONS = "Permissões insuficientes para esta operação";
    public const string AUTH_VALIDATION_ERROR = "Erro de validação nos dados de autenticação";
    public const string AUTH_USER_BLOCKED = "Usuário bloqueado";
    public const string AUTH_USER_LOCKED_OUT = "Usuário bloqueado por tentativas excessivas";

    // File Messages
    public const string FILE_INVALID_FORMAT = "Formato de arquivo inválido";
    public const string FILE_TOO_LARGE = "Arquivo muito grande";
    public const string FILE_UPLOAD_FAILED = "Falha no upload do arquivo";
}
