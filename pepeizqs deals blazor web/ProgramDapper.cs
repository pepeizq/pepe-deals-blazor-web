#nullable disable

using Bundles2;
using Dapper;
using Juegos;
using System.Data;
using System.Text.Json;

public class JuegoHandler<T> : SqlMapper.TypeHandler<T>
{
	public override T Parse(object valor)
	{
		if (valor == null || valor == DBNull.Value)
		{
			return default;
		}

		string texto = valor.ToString();
		if (string.IsNullOrWhiteSpace(texto) || texto == "null")
		{
			return default;
		}

		return JsonSerializer.Deserialize<T>(texto);
	}

	public override void SetValue(IDbDataParameter parametro, T valor)
	{
		parametro.Value = valor == null ? (object)DBNull.Value : JsonSerializer.Serialize(valor);
	}
}

public class BundleHandler<T> : SqlMapper.TypeHandler<List<T>>
{
	public override List<T> Parse(object valor)
	{
		if (valor == null || valor == DBNull.Value)
		{
			return default;
		}

		string texto = valor.ToString();
		if (string.IsNullOrWhiteSpace(texto) || texto == "null")
		{
			return default;
		}

		return Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(texto);
	}

	public override void SetValue(IDbDataParameter parametro, List<T> valor)
	{
		parametro.Value = valor == null ? (object)DBNull.Value : Newtonsoft.Json.JsonConvert.SerializeObject(valor);
	}
}

public class JuegoDRMHandler : SqlMapper.TypeHandler<JuegoDRM>
{
	public override JuegoDRM Parse(object valor)
	{
		return JuegoDRM2.DevolverDRM((int)valor);
	}

	public override void SetValue(IDbDataParameter parametro, JuegoDRM valor)
	{
		parametro.Value = (int)valor;
	}
}

public static class ClasesDapper
{
	public static void Registrar()
	{
		SqlMapper.AddTypeHandler(new JuegoHandler<JuegoImagenes>());
		SqlMapper.AddTypeHandler(new JuegoHandler<List<JuegoPrecio>>());
		SqlMapper.AddTypeHandler(new JuegoHandler<JuegoAnalisis>());
		SqlMapper.AddTypeHandler(new JuegoHandler<JuegoCaracteristicas>());
		SqlMapper.AddTypeHandler(new JuegoHandler<JuegoMedia>());
		SqlMapper.AddTypeHandler(new JuegoHandler<JuegoMediaVideo>());
		SqlMapper.AddTypeHandler(new JuegoHandler<List<JuegoBundle>>());
		SqlMapper.AddTypeHandler(new JuegoHandler<List<JuegoGratis>>());
		SqlMapper.AddTypeHandler(new JuegoHandler<List<JuegoSuscripcion>>());
		SqlMapper.AddTypeHandler(new JuegoHandler<List<string>>());
		SqlMapper.AddTypeHandler(new JuegoHandler<List<JuegoDeckToken>>());
		SqlMapper.AddTypeHandler(new JuegoHandler<List<JuegoHistorico>>());
		SqlMapper.AddTypeHandler(new JuegoHandler<JuegoGalaxyGOG>());
		SqlMapper.AddTypeHandler(new JuegoHandler<JuegoCantidadJugadoresSteam>());
		SqlMapper.AddTypeHandler(new JuegoHandler<List<JuegoIdioma>>());
		SqlMapper.AddTypeHandler(new JuegoHandler<JuegoEpicGames>());
		SqlMapper.AddTypeHandler(new JuegoHandler<JuegoXbox>());
		SqlMapper.AddTypeHandler(new BundleHandler<BundleJuego>());
		SqlMapper.AddTypeHandler(new BundleHandler<BundleTier>());
		SqlMapper.AddTypeHandler(new JuegoDRMHandler());
	}
}