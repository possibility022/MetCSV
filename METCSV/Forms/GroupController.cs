using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace METCSV.Forms
{
    public partial class GroupController : Form
    {
        List<Product> productsWithoutGroup = new List<Product>();
        List<Generator.ProductGroup> productGroup = new List<Generator.ProductGroup>();
        
        public bool allSolved { get; private set; }

        public GroupController()
        {
            InitializeComponent();
        }

        public void LoadGroups(List<Generator.ProductGroup> groups, List<Product> withoutGroups)
        {
            productsWithoutGroup = withoutGroups;
            productGroup = groups;

            solveUsingAI();

            if (groups != null)
            for (int i = 0; i < groups.Count; i++)
            {
                lvGroups.Items.Add(new ListViewItemGroup(i.ToString(), groups[i]));
            }

            for(int i = 0; i < productsWithoutGroup.Count; i++)
            {
                lvProductsToSolve.Items.Add(new ListViewItemProduct(productsWithoutGroup[i].NazwaProduktu, productsWithoutGroup[i]));
            }

            allSolved = productsWithoutGroup.Count == 0;

        }

        private void lvGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            lvProductsInGroup.Clear();
            try
            {
                List<Product> list = productGroup[lvGroups.SelectedItems[0].Index].getList();

                for (int i = 0; i < list.Count; i++)
                {
                    lvProductsInGroup.Items.Add(new ListViewItemProduct(list[i].NazwaProduktu, list[i]));
                }

            }catch(Exception ex)
            {

            }
        }

        public List<Generator.ProductGroup> getGroups()
        {
            if (this.productGroup == null)
                this.productGroup = new List<Generator.ProductGroup>();
            return this.productGroup;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            moveToGroup();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            addGroup();
        }

        private void solveUsingAI()
        {
            if (productGroup == null)
                return;

            for (int i = 0; i < productsWithoutGroup.Count; i++)
            {
                foreach (Generator.ProductGroup gr in productGroup)
                {
                    if (gr.updateProductIfInGroup(productsWithoutGroup[i]))
                    {
                        productsWithoutGroup.RemoveAt(i);
                        i--;
                        break;
                    }
                }
            }

            moveIfSameManufacturer();
        }

        private void moveIfSameManufacturer()
        {
            if (productGroup == null)
                productGroup = new List<Generator.ProductGroup>();

            for(int i = 0; i < productsWithoutGroup.Count; i++)
            {
                for(int groupIndex = 0; groupIndex < productGroup.Count; groupIndex++)
                {
                    if (productGroup[groupIndex].manufacturerInGroup(productsWithoutGroup[i].NazwaProducenta))
                    {
                        productGroup[groupIndex].addProduct(productsWithoutGroup[i]);
                        productsWithoutGroup.RemoveAt(i);
                        i--;
                        break;
                    }
                }

                Generator.ProductGroup newGroup = new Generator.ProductGroup();
                newGroup.addProduct(productsWithoutGroup[i]);
                productsWithoutGroup.RemoveAt(i);
                i--;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            removeFromGroup();
        }

        private void addGroup(string name = "0")
        {
            if (name == "0")
                name = lvGroups.Items.Count + 1.ToString();

            foreach(ListViewItem item in lvGroups.Items)
            {
                item.Selected = false;
            }

            if (productGroup == null) productGroup = new List<Generator.ProductGroup>();
            Generator.ProductGroup group = new Generator.ProductGroup();
            lvGroups.Items.Add(new ListViewItemGroup(name, group));
            productGroup.Add(group);
            lvGroups.Items[lvGroups.Items.Count - 1].Selected = true;
            selectedGroupChanged();
        }

        private void moveToGroup()
        {
            if (lvGroups.Items.Count == 0)
                addGroup();

            try
            {
                ListViewItemProduct listViewItem = (ListViewItemProduct)lvProductsToSolve.SelectedItems[0];
                productGroup[lvGroups.SelectedItems[0].Index].addProduct(listViewItem.product);

                lvProductsToSolve.Items.Remove(listViewItem);
                lvProductsInGroup.Items.Add(listViewItem);

                productsWithoutGroup.Remove(listViewItem.product);

                if (lvProductsToSolve.Items.Count > 0)
                    lvProductsToSolve.Items[0].Selected = true;
            }
            catch (Exception ex)
            { }
        }

        private void removeFromGroup()
        {
            try
            {
                ListViewItemProduct listViewItem = (ListViewItemProduct)lvProductsInGroup.SelectedItems[0];
                productsWithoutGroup.Add(listViewItem.product);

                lvProductsInGroup.Items.Remove(listViewItem);
                lvProductsToSolve.Items.Add(listViewItem);

                productGroup[lvGroups.SelectedItems[0].Index].removeProduct(listViewItem.product);
            }
            catch (Exception ex)
            {

            }
        }

        private void selectedGroupChanged()
        {
            try
            {
                ListViewItemProduct lvp = (ListViewItemProduct)lvProductsInGroup.SelectedItems[0];
                productViewer1.loadProduct(lvp.product);
            }
            catch (Exception ex)
            {
                productViewer1.clear();
            }
        }

        private void lvProductsToSolve_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ListViewItemProduct lvp = (ListViewItemProduct)lvProductsToSolve.SelectedItems[0];
                productViewer2.loadProduct(lvp.product);
            }
            catch(Exception ex)
            {
                productViewer2.clear();
            }
        }

        private void lvProductsInGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedGroupChanged();
        }

        private void lvProductsToSolve_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            moveToGroup();
        }

        private void lvProductsInGroup_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            removeFromGroup();
        }
    }

    public class ListViewItemGroup : ListViewItem
    {
        Generator.ProductGroup group { get; set; }

        public ListViewItemGroup(string value, Generator.ProductGroup group) : base(value)
        {
            this.group = group;
        }
    }

    public class ListViewItemProduct : ListViewItem
    {
        public Product product { get; set; }

        public ListViewItemProduct(string value, Product product) : base(value)
        {
            this.product = product;
        }
    }
}
