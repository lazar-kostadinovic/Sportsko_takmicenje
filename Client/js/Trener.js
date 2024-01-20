document.addEventListener("DOMContentLoaded", async function () {
  const listaTrenera = document.getElementById("listaTrenera");
  const kreirajForma = document.getElementById("kreirajForma");
  const izmeniForma = document.getElementById("izmeniForma");
  const izbrisiForma = document.getElementById("izbrisiForma");
  const sportForma = document.getElementById("sportForma");
  const timForma = document.getElementById("timForma");
  const sportSelect = document.getElementById("sportSelect");
  const timSelect = document.getElementById("timSelect");
  displayTreneriList();
  displayTimList();
  displaySports();

  async function displayTimList() {
    timSelect.innerHTML = "";
    const timovi = await getTimovi();

    timovi.forEach((tim) => {
      const option = document.createElement("option");
      option.value = tim.id;
      option.text = tim.naziv;
      timSelect.appendChild(option);
    });
  }

  async function getTimovi() {
    const response = await fetch("http://localhost:5064/api/Tim/get-all-teams");
    const data = await response.json();
    return data;
  }

  async function displaySports() {
    const sportovi = await getSportovi();
    sportovi.forEach((sport) => {
      const option = document.createElement("option");
      option.value = sport.sportId;
      option.text = sport.naziv;
      sportSelect.appendChild(option);
    });
  }

  async function getSportovi() {
    const response = await fetch(
      "http://localhost:5064/api/Sport/get-all-sportovi"
    );
    const data = await response.json();
    return data;
  }

  async function dodajTreneraUSport(jmbg, sportId) {
    const response = await fetch(
      `http://localhost:5064/api/Trener/dodaj-sport-treneru/${jmbg}/${sportId}`,
      {
        method: "POST",
      }
    );
    if (response.status > 299) {
      window.alert("GRESKA");
      return;
    }
    window.alert("Dodato!");
    displayTreneriList();
  }

  async function dodajTreneraUTim(jmbg, timId) {
    const response = await fetch(
      `http://localhost:5064/api/Trener/dodaj-tim-treneru/${jmbg}/${timId}`,
      {
        method: "POST",
      }
    );
    if (response.status > 299) {
      window.alert("GRESKA");
      return;
    }
    window.alert("Dodato!");
    displayTreneriList();
  }

  async function displayTreneriList() {
    listaTrenera.innerHTML = "";
    const treneri = await getTreneri();

    const headerRow = document.createElement("tr");
    for (let key in treneri[0]) {
      if (key !== "sport" && key !== "tim") {
        const th = document.createElement("th");
        th.innerHTML = key;
        headerRow.append(th);
      }
    }
    listaTrenera.append(headerRow);

    treneri.forEach((trener) => {
      const tr = document.createElement("tr");
      for (let key in trener) {
        if (key !== "sport" && key !== "tim") {
          const td = document.createElement("td");
          td.innerHTML = trener[key];
          tr.append(td);
        }
      }
      listaTrenera.append(tr);
    });
  }

  async function getTreneri() {
    const response = await fetch(
      "http://localhost:5064/api/Trener/get-all-trener"
    );
    const data = await response.json();
    return data;
  }

  async function getTreneri() {
    const response = await fetch(
      "http://localhost:5064/api/Trener/get-all-trener"
    );
    const data = await response.json();
    return data;
  }

  async function kreairajTrenera(trener) {
    const response = await fetch(
      "http://localhost:5064/api/Trener/create-trener",
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(trener),
      }
    );
    if (response.status != 200) {
      window.alert("GRESKA!");
      return;
    }
    const data = await response.json();
    displayTreneriList();
    window.alert("Trener dodat: " + JSON.stringify(data));
  }

  async function izmeniTrenera(jmbg, adresa) {
    const response = await fetch(
      `http://localhost:5064/api/Trener/put-trener/${jmbg}/${adresa}`,
      {
        method: "PUT",
      }
    );
    if (response.status != 200) {
      window.alert(JSON.stringify(await response.json()));
      return;
    }
    const data = await response.json();
    displayTreneriList();
    window.alert("Trener izmenjen: " + JSON.stringify(data));
  }

  async function izbrisiTrenera(jmbg) {
    const response = await fetch(
      `http://localhost:5064/api/Trener/delete-trener/${jmbg}`,
      {
        method: "DELETE",
      }
    );
    if (response.status > 205) {
      window.alert(JSON.stringify(await response.json()));
      return;
    }
    displayTreneriList();
  }

  kreirajForma.addEventListener("submit", async function (event) {
    event.preventDefault();
    const formData = new FormData(kreirajForma);
    const trener = {
      jmbg: formData.get("jmbg"),
      ime: formData.get("ime"),
      prezime: formData.get("prezime"),
      datumRodjenja: formData.get("datumRodjenja"),
      brojTelefona: formData.get("brojTelefona"),
      pol: formData.get("pol"),
      adresa: formData.get("adresa"),
      drzava: formData.get("drzava"),
    };
    kreairajTrenera(trener);
  });
  izmeniForma.addEventListener("submit", async function (event) {
    event.preventDefault();
    const formData = new FormData(izmeniForma);
    izmeniTrenera(formData.get("jmbg"), formData.get("adresa"));
  });

  izbrisiForma.addEventListener("submit", async function (event) {
    event.preventDefault();
    const formData = new FormData(izbrisiForma);
    izbrisiTrenera(formData.get("jmbg"));
  });

  sportForma.addEventListener("submit", async function (event) {
    event.preventDefault();
    const formData = new FormData(sportForma);
    dodajTreneraUSport(formData.get("jmbg"), formData.get("sportId"));
  });

  timForma.addEventListener("submit", async function (event) {
    event.preventDefault();
    const formData = new FormData(timForma);
    dodajTreneraUTim(formData.get("jmbg"), formData.get("timId"));
  });
});
