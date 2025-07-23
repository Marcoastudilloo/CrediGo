namespace CrediGo.Utils
{

    public static class CalculadoraCredito
    {
        public static decimal CalcularPagoMensual(decimal monto, decimal tasaInteresAnual, int plazoMeses)
        {
            if (tasaInteresAnual <= 0 || plazoMeses <= 0)
                return 0;

            var tasaMensual = (double)tasaInteresAnual / 12 / 100;
            var montoDouble = (double)monto;

            double pago = montoDouble * tasaMensual * Math.Pow(1 + tasaMensual, plazoMeses)
                          / (Math.Pow(1 + tasaMensual, plazoMeses) - 1);

            return Math.Round((decimal)pago, 2);
        }
    }
}