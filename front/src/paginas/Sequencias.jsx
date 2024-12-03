import "./componentes.css";
import axios from "axios";
import Select from "react-select";
import { useState, useEffect } from "react";

//npm install react-select
//Estilos do componente react-select
const selectStyles = {
  control: (baseStyles, state) => ({
    ...baseStyles,
    margin: 0,
    borderRadius: 3,
    borderColor: "gray",
    boxShadow: state.isFocused ? "0 0 0 2px black" : 0,
    ":hover": { borderColor: "black" },
  }),
};

function Sequencias() {
  //Entidades e listas utilizadas na página
  const [individuos, setIndividuos] = useState([]);
  const [individuo, setIndividuo] = useState(null);
  const [sequencias, setSequencias] = useState([]);
  const [sequencia, setSequencia] = useState(null);

  //Funções de carregamento de dados do backend
  function getSequencias() {
    axios.get("http://localhost:5224/sequencias").then((resposta) => {
      setSequencias(resposta.data);
    });
  }

  function getIndividuos() {
    axios.get("http://localhost:5224/individuos").then((resposta) => {
      //setIndividuos(resposta.data);

      const valores = [];
      const individuos = resposta.data;
      for (let i = 0; i < individuos.length; i++) {
        const vlr = individuos[i];
        valores[i] = {
          id: vlr.id,
          codigo: vlr.codigo,
          nome: vlr.nome,
          value: vlr.id,
          label: vlr.codigo + ", " + vlr.nome,
        };
      }
      setIndividuos(valores);
    });
  }

  useEffect(() => {
    getIndividuos();
    getSequencias();
  }, []);

  function novaSequencia() {
    setSequencia({ id: 0, seqGenetica: "", individuo: null });
  }

  function editarSequencia(sequencia) {
    setSequencia(sequencia);
    const individuo = sequencia.individuo;
    setIndividuo({
      id: individuo.id,
      codigo: individuo.codigo,
      nome: individuo.nome,
      value: individuo.id,
      label: individuo.codigo + ", " + individuo.nome,
    });
  }

  function alterarSequencia(campo, valor) {
    sequencia[campo] = valor;
    setSequencia({
      ...sequencia,
    });
  }

  function excluirSequencia(id) {
    axios
      .delete(`http://localhost:5224/sequencias/${id}`)
      .then(() => {
        reiniciarEstadoDosObjetos(); // Atualiza a lista após exclusão
      })
      .catch((erro) => {
        console.error("Erro ao excluir a sequência:", erro);
        alert("Não foi possível excluir a sequência.");
      });
  }

  function salvarSequencia() {
    if (sequencia.id > 0) {
      axios
        .put("http://localhost:5224/sequencias/" + sequencia.id, sequencia)
        .then(() => {
          reiniciarEstadoDosObjetos();
        });
    } else {
      axios.post("http://localhost:5224/sequencias", sequencia).then(() => {
        reiniciarEstadoDosObjetos();
      });
    }
  }

  function reiniciarEstadoDosObjetos() {
    setIndividuo(null);
    setSequencia(null);
    getSequencias();
  }

  //Caixa de seleção de INDIVÍDUOS
  function getSelectIndividuos() {
    return (
      <Select
        isClearable={false}
        value={individuo}
        onChange={onChangeSelectIndividuo}
        options={individuos}
        styles={selectStyles}
      />
    );
  }

  function onChangeSelectIndividuo(value) {
    console.log(value);
    setIndividuo({
      ...value,
    });
    setSequencia({
      ...sequencia,
      individuo: value,
    });
  }

  //Função para geração do formulário
  function getFormulario() {
    return (
      <form>
        <label>Indivíduo</label>
        {getSelectIndividuos()}
        <label>Sequência</label>
        <input
          type="text"
          name="seqGenetica"
          value={sequencia?.seqGenetica || ""}
          onChange={(e) => {
            alterarSequencia(e.target.name, e.target.value);
          }}
        />
        <button
          type="button"
          onClick={() => {
            salvarSequencia();
          }}
        >
          Salvar
        </button>
        <button
          type="button"
          onClick={() => {
            reiniciarEstadoDosObjetos();
          }}
        >
          Cancelar
        </button>
      </form>
    );
  }

  //Funções para geração da tabela
  function getLinhaDaTabela(sequencia) {
    const individuoSeq = sequencia.individuo;
    return (
      <tr key={sequencia.id}>
        <td>{sequencia.id}</td>
        <td>{individuoSeq.codigo + ", " + individuoSeq.nome}</td>
        <td>{sequencia.seqGenetica}</td>
        <td>
          <button
            type="button"
            onClick={() => {
              if (
                window.confirm(
                  "Confirmar a exclusão da sequência " +
                    sequencia.sequencia +
                    "?"
                )
              ) {
                excluirSequencia(sequencia.id);
              }
            }}
          >
            Excluir
          </button>
          <button
            type="button"
            onClick={() => {
              editarSequencia(sequencia);
            }}
          >
            Editar
          </button>
        </td>
      </tr>
    );
  }

  function getLinhasDaTabela() {
    const linhasDaTabela = [];
    for (let i = 0; i < sequencias.length; i++) {
      const sequencia = sequencias[i];
      if (sequencia.individuo) {
        linhasDaTabela[i] = getLinhaDaTabela(sequencia);
      }
    }
    return linhasDaTabela;
  }

  function getTabela() {
    return (
      <table key="tabSequencias">
        <tbody>
          <tr key="hedSequencias">
            <th>ID</th>
            <th>Indivíduo</th>
            <th>Sequência</th>
            <th>Ações</th>
          </tr>
          {getLinhasDaTabela()}
        </tbody>
      </table>
    );
  }

  //Função do conteúdo principal
  function getConteudo() {
    if (sequencia === null) {
      return (
        <>
          <button
            type="button"
            onClick={() => {
              novaSequencia();
            }}
          >
            Nova sequência
          </button>
          {getTabela()}
        </>
      );
    } else {
      return getFormulario();
    }
  }

  return (
    <div className="cadastros">
      <div className="conteudo">{getConteudo()}</div>
    </div>
  );
}

export default Sequencias;
